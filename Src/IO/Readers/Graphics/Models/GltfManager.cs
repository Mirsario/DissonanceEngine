using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dissonance.Engine.Graphics;
using Newtonsoft.Json;

namespace Dissonance.Engine.IO
{
	public partial class GltfReader : IAssetReader<PackedScene>
	{
		public const uint FormatHeader = 0x46546C67;

		private const uint ChunkJson = 0x4E4F534A;
		private const uint ChunkBinary = 0x004E4942;

		public static readonly Dictionary<ComponentType, uint> ComponentTypeSizes = new() {
			{ ComponentType.SByte,  1 },
			{ ComponentType.Byte,   1 },
			{ ComponentType.Short,  2 },
			{ ComponentType.UShort, 2 },
			{ ComponentType.UInt,   4 },
			{ ComponentType.Float,  4 },
		};
		public static readonly Dictionary<string, uint> AccessorTypeSizes = new(StringComparer.InvariantCultureIgnoreCase) {
			{ "SCALAR", 1 },
			{ "VEC2",   2 },
			{ "VEC3",   3 },
			{ "VEC4",   4 },
			{ "MAT2",   4 },
			{ "MAT3",   9 },
			{ "MAT4",   16 },
		};
		public static readonly Dictionary<string, Type> AttributeToType = new(StringComparer.InvariantCultureIgnoreCase) {
			{ "POSITION",       typeof(VertexAttribute) },
			{ "NORMAL",         typeof(NormalAttribute) },
			{ "TANGENT",        typeof(TangentAttribute) },
			{ "TEXCOORD_0",     typeof(Uv0Attribute) },
			// { "TEXCOORD_1",	typeof(Uv1Attribute) },
			{ "COLOR_0",        typeof(ColorAttribute) },
			// { "JOINTS_0",		typeof(BoneIndicesAttribute) },
			// { "WEIGHTS_0",	typeof(BoneWeightsAttribute) },
		};

		public string[] Extensions { get; } = { ".gltf", ".glb" };

		public PackedScene ReadFromStream(Stream stream, string filePath)
		{
			var info = new GltfInfo(filePath);

			if (filePath.EndsWith(".gltf")) {
				byte[] textBytes = new byte[stream.Length - stream.Position];

				stream.Read(textBytes, 0, textBytes.Length);

				HandleGltf(info, textBytes);
			} else {
				HandleGlb(info, stream);
			}

			//TODO: Add extension support.
			if (info.json.extensionsRequired != null) {
				foreach (string requiredExtension in info.json.extensionsRequired) {
					throw new FileLoadException($"glTF Error: Required extension '{requiredExtension}' is not supported.");
				}
			}

			LoadMeshes(info);

			return info.Scene;
		}

		protected static void HandleGltf(GltfInfo info, byte[] textBytes)
		{
			info.json = JsonConvert.DeserializeObject<GltfJson>(Encoding.UTF8.GetString(textBytes));

#if DEBUG
			Directory.CreateDirectory("DebugInfo");
			File.WriteAllText(
				Path.Combine("DebugInfo", "Gltf.json"),
				JsonConvert.SerializeObject(info.json, Formatting.Indented, new JsonSerializerSettings() {
					DefaultValueHandling = DefaultValueHandling.Ignore
				})
			);
#endif
		}
		protected static void HandleGlb(GltfInfo info, Stream stream)
		{
			using var reader = new BinaryReader(stream);

			ReadHeader(reader, out _, out uint length);

			int chunkId = 0;

			while (reader.BaseStream.Position < length) {
				// Read Chunk
				uint chunkLength = reader.ReadUInt32();
				uint chunkType = reader.ReadUInt32();
				byte[] chunkData = reader.ReadBytes((int)chunkLength);

				if (chunkId == 0 && chunkType != ChunkJson) {
					static string ToAscii(uint value) => Encoding.ASCII.GetString(BitConverter.GetBytes(value));

					throw new FileLoadException($"glTF Error: First chunk was expected to be '{ToAscii(ChunkJson)}', but is '{ToAscii(chunkType)}'.");
				}

				switch (chunkType) {
					case ChunkJson:
						HandleGltf(info, chunkData);
						break;
					case ChunkBinary:
						info.blobStream = new MemoryStream(chunkData);
						break;
				}

				chunkId++;
			}
		}

		protected static void ReadHeader(BinaryReader reader, out uint version, out uint length)
		{
			uint magic = reader.ReadUInt32();

			if (magic != FormatHeader) {
				throw new FileLoadException("glTF Error: File is not of 'Binary glTF' format.");
			}

			version = reader.ReadUInt32();
			length = reader.ReadUInt32();
		}

		protected static byte[] GetAccessorData(GltfInfo info, GltfJson.Accessor accessor)
		{
			var json = info.json;
			var bufferView = accessor.bufferView.HasValue ? json.bufferViews[accessor.bufferView.Value] : null;

			int elementSize = (int)AccessorTypeSizes[accessor.type];
			int packSize = (int)(elementSize * ComponentTypeSizes[accessor.componentType]);
			int fullSize = (int)(bufferView?.byteLength ?? accessor.count * packSize);

			byte[] data = new byte[fullSize];

			if (bufferView != null) {
				uint bufferId = bufferView.buffer;
				var buffer = json.buffers[bufferId];

				Stream stream;

				if (buffer.uri == null) {
					// Read from blob.

					if (info.blobStream == null) {
						throw new FileLoadException($"glTF Error: Buffer {bufferId} is missing 'uri' property, with no binary buffer present.");
					}

					if (bufferId != 0) {
						throw new FileLoadException($"glTF Error: Buffer {bufferId} is missing 'uri' property. Only the first buffer in a .glb file is allowed to not have one.");
					}

					stream = info.blobStream;
				} else {
					// Read from uri.

					string path = buffer.uri;

					// If uri/path is not absolute, we make it relative to the .gltf file's location.
					if (!buffer.uri.Contains(":/") && !buffer.uri.Contains(":\\") && !buffer.uri.StartsWith("/")) {
						path = Path.GetRelativePath(Directory.GetCurrentDirectory(), Path.GetDirectoryName(info.FilePath)) + Path.DirectorySeparatorChar + buffer.uri;
					}

					stream = File.OpenRead(path);
				}

				if (!stream.CanSeek) {
					throw new FileLoadException("glTF Error: Stream is not seekable. This shouldn't happen.");
				}

				stream.Seek(bufferView.byteOffset + accessor.byteOffset, SeekOrigin.Begin);

				// if (bufferView.byteStride==0) {
				stream.Read(data, 0, (int)bufferView.byteLength);
				// } else {
				//	int bytesRead = 0;
				//
				//	while (bytesRead<bufferView.byteLength) {
				//		stream.Read(data,bytesRead,elementSize);
				//
				//		bytesRead += elementSize;
				//
				//		stream.Seek(bufferView.byteStride,SeekOrigin.Current);
				//	}
				// }
			}

			return data;
		}

		private static void LoadMeshes(GltfInfo info)
		{
			var json = info.json;

			if (json.meshes == null || json.meshes.Length == 0) {
				return;
			}

			foreach (var jsonMesh in json.meshes) {
				var meshes = new List<Asset<Mesh>>();

				foreach (var jsonPrimitive in jsonMesh.primitives) {
					var mesh = new Mesh();

					if (jsonPrimitive.mode.HasValue) {
						mesh.PrimitiveType = jsonPrimitive.mode.Value;
					}

					if (jsonPrimitive.indices.HasValue) {
						var jsonAccessor = json.accessors[jsonPrimitive.indices.Value];

						byte[] data = GetAccessorData(info, jsonAccessor);

						mesh.IndexBuffer.SetData<ushort>(data, value => value);
					}

					foreach (var pair in jsonPrimitive.attributes) {
						string attributeName = pair.Key;
						var jsonAccessor = json.accessors[pair.Value];

						if (!AttributeToType.TryGetValue(attributeName, out var attributeType)) {
							continue;
						}

						var attribute = CustomVertexAttribute.GetInstance(attributeType);
						var buffer = mesh.GetBuffer(attribute.BufferType);

						byte[] data = GetAccessorData(info, jsonAccessor);

						buffer.SetData(data);
					}

					mesh.Apply();

					meshes.Add(Assets.CreateUntracked(mesh.Name, mesh));
				}

				//TODO: Add proper material support, remove this hardcode.
				var material = Assets.CreateUntracked("DefaultMaterial", new Material("DefaultMaterial", Rendering.RenderingPipeline.DefaultGeometryShader));

				for (int i = 0; i < meshes.Count; i++) {
					var entity = info.Scene.CreateEntity();

					entity.SetComponent(new Transform(Vector3.Zero));
					entity.SetComponent(new MeshRenderer(meshes[i], material));
				}

				// info.result.Add(model, jsonMesh.name);
			}
		}
	}
}
