using System;
using System.IO;
using Ionic.Zlib;

namespace GameEngine
{
	public class MeshManager : AssetManager<Mesh>
	{
		public override string[] Extensions => new[] { ".mesh" };
		
		public override Mesh Import(Stream stream,string fileName)
		{
			var reader = new BinaryReader(stream);
			
			uint chunkAmount = reader.ReadUInt32();
			uint vertexCount = reader.ReadUInt32();
			uint triangleCount = reader.ReadUInt32();
			uint triIndiceCount = triangleCount*3;

			Debug.Log("vertexCount: "+vertexCount);
			Debug.Log("triangleCount: "+triangleCount);
			
			var vertices = new Vector3[vertexCount];
			var triangles = new int[triIndiceCount];
			Vector2[] uv0 = null;
			Vector3[] normals = null;
			Vector4[] tangents = null;
			Vector4[] colors = null;
			BoneWeight[] boneWeights = null;
			AnimationSkeleton skeleton = null;
			
			for(int chunk = 0;chunk<chunkAmount;chunk++) {
				ReadChunk(reader,(chunkName,r) => {
					switch(chunkName) {
						#region Vertices
						case "vertices": {
							for(int i = 0;i<vertexCount;i++) {
								vertices[i] = r.ReadVector3();
								//Debug.Log($"Vertices[{i}]=={vertices[i]}");
							}
							break;
						}
						#endregion
						#region Triangles
						case "triangles": {
							for(int i = 0;i<triIndiceCount;i++) {
								triangles[i] = r.ReadInt32();
								//Debug.Log($"triangles[{i}]=={triangles[i]}");
							}
							break;
						}
						#endregion
						#region Normals
						case "normals": {
							normals = new Vector3[vertexCount];
							for(int i = 0;i<vertexCount;i++) {
								normals[i] = r.ReadVector3();
							}
							break;
						}
						#endregion
						#region Tangents
						case "tangents": {
							tangents = new Vector4[vertexCount];
							for(int i = 0;i<vertexCount;i++) {
								tangents[i] = r.ReadVector4();
							}
							break;
						}
						#endregion
						#region UVs
						case "uv0": {
							uv0 = new Vector2[vertexCount];
							for(int i = 0;i<vertexCount;i++) {
								uv0[i] = new Vector2(r.ReadSingle(),r.ReadSingle());
							}
							break;
						}
						#endregion
						#region BoneWeights
						case "boneWeights": {
							boneWeights = new BoneWeight[vertexCount];
							for(int i = 0;i<vertexCount;i++) {
								boneWeights[i].boneIndex0 = r.ReadInt32();
								boneWeights[i].boneIndex1 = r.ReadInt32();
								boneWeights[i].boneIndex2 = r.ReadInt32();
								boneWeights[i].boneIndex3 = r.ReadInt32();
								boneWeights[i].weight0 = r.ReadSingle();
								boneWeights[i].weight1 = r.ReadSingle();
								boneWeights[i].weight2 = r.ReadSingle();
								boneWeights[i].weight3 = r.ReadSingle();
							}
							break;
						}
							#endregion
						#region Skeleton
						/*case "skeleton": {
							Debug.Log("Reading skeleton chunk");
							skeleton = new Skeleton(r.ReadString());
							int bonesCount = r.ReadInt32();
							skeleton.bones = new Bone[bonesCount];
							int[] parentIds = new int[bonesCount];
							for(int i = 0;i<bonesCount;i++) {
								skeleton.bones[i] = new Bone(r.ReadString());
								Vector3 localPosition = r.ReadVector3();
								Quaternion localRotation = r.ReadQuaternion();
								Vector3 localScale = r.ReadVector3();
								skeleton.bones[i].baseMatrix = Matrix4x4.CreateTranslation(localPosition)*Matrix4x4.CreateScale(localScale)*Matrix4x4.CreateRotation(localRotation);
								parentIds[i] = r.ReadInt32();
								//Debug.Log(parentIds[i]);
								//Transform testTransform = new Transform();
								//testTransform.localPosition = localPosition*scale;
								//testTransform.localRotation = localRotation;
								//testTransform.localScale = localScale;
								//skeleton.bones[i].baseMatrix = testTransform.matrix;
								//testTransform = null;
								//	Matrix4x4.CreateScale(localScale*scale)*
								//	Matrix4x4.CreateTranslation(localPosition*scale)*
								//	Matrix4x4.CreateRotation(localRotation);
							}
							for(int i = 0;i<bonesCount;i++) {
								int id = parentIds[i];
								if(id>=0) {
									skeleton.bones[i].parent = skeleton.bones[id];
								}
							}
							break;
						}*/
						#endregion
					}
				});
			}
			reader.Close();
			stream.Close();

			var mesh = new Mesh {
				name = fileName ?? "UntitledMesh",
				vertices = vertices,
				triangles = triangles,
				uv = uv0,
				normals = normals,
				tangents = tangents,
				colors = colors,
				boneWeights = boneWeights
			};

			//skeleton = skeleton

			//if(normals==null) {
			//	mesh.RecalculateNormals();
			//}

			mesh.Apply();

			return mesh;
		}
		public override void Export(Mesh mesh,Stream stream)
		{
			var writer = new BinaryWriter(stream);

			int chunkAmount = 0;
			long amountPos = writer.BaseStream.Position;

			writer.Write(0); //32 bit int to be rewritten at the end

			int vertexCount = mesh.vertices.Length;
			writer.Write(vertexCount);

			int triangleCount = mesh.triangles.Length;
			writer.Write(triangleCount);

			void WriteChunk(string name,Action<BinaryWriter> writeAction)
			{
				MeshManager.WriteChunk(writer,name,writeAction);
				chunkAmount++;
			}

			//Vertices
			WriteChunk("vertices",w => {
				for(int i = 0;i<vertexCount;i++) {
					w.Write(mesh.vertices[i]);
				}
			});

			//Triangles
			WriteChunk("triangles",w => {
				for(int i = 0;i<triangleCount;i++) {
					w.Write(mesh.triangles[i]);
				}
			});

			//UV
			if(mesh.uv?.Length>0==true) {
				WriteChunk("uv0",w => {
					for(int i = 0;i<vertexCount;i++) {
						w.Write(mesh.uv[i]);
					}
				});
			}

			//Normals
			if(mesh.normals?.Length>0==true) {
				WriteChunk("normals",w => {
					for(int i = 0;i<vertexCount;i++) {
						w.Write(mesh.normals[i]);
					}
				});
			}

			//Tangents
			if(mesh.tangents?.Length>0==true) {
				WriteChunk("tangents",w => {
					for(int i = 0;i<vertexCount;i++) {
						w.Write(mesh.tangents[i]);
					}
				});
			}

			//VertexColor
			if(mesh.colors?.Length>0==true) {
				WriteChunk("colors",w => {
					for(int i = 0;i<vertexCount;i++) {
						var c = mesh.colors[i];

						w.Write((byte)(c.x*255));
						w.Write((byte)(c.y*255));
						w.Write((byte)(c.z*255));
						w.Write((byte)(c.w*255));
					}
				});
			}

			//BoneWeights
			if(mesh.boneWeights?.Length>0==true) {
				WriteChunk("boneWeights",w => {
					for(int i = 0;i<vertexCount;i++) {
						var weight = mesh.boneWeights[i];

						w.Write(weight.boneIndex0);
						w.Write(weight.boneIndex1);
						w.Write(weight.boneIndex2);
						w.Write(weight.boneIndex3);

						w.Write(weight.weight0);
						w.Write(weight.weight1);
						w.Write(weight.weight2);
						w.Write(weight.weight3);
					}
				});
			}

			long prevPos = writer.BaseStream.Position;

			writer.BaseStream.Position = amountPos;
			writer.Write(chunkAmount);
			writer.BaseStream.Position = prevPos;

			writer.Dispose();
		}

		public static void WriteChunk(BinaryWriter writer,string name,Action<BinaryWriter> action)
		{
			writer.Write((byte)name.Length);
			writer.Write(name.ToCharArray());
			var output = new MemoryStream();
			using(var deflateStream = new DeflateStream(output,CompressionMode.Compress,CompressionLevel.BestCompression,false)) {
				using(var newWriter = new BinaryWriter(deflateStream)) {
					action(newWriter);
				}
			}
			var data = output.ToArray();
			writer.Write((ulong)data.LongLength);
			writer.Write(data);
		}
		public static void ReadChunk(BinaryReader reader,Action<string,BinaryReader> action)
		{
			string chunkName = new string(reader.ReadChars(reader.ReadByte()));//reader.ReadString();//
			ulong length = reader.ReadUInt64();
			var input = new MemoryStream(reader.ReadBytes((int)length));//Shouldn't there be an overload which'd take a long?
			var output = new MemoryStream();
			using(var stream = new DeflateStream(input,CompressionMode.Decompress)) {
				stream.CopyTo(output);
			}
			output.Position = 0;
			using(var newReader = new BinaryReader(output)) {
				action(chunkName,newReader);
			}
			output?.Dispose();
		}

		/*public static byte[] CompressBytes(byte[] data)
		{
			MemoryStream output = new MemoryStream();
			using(var deflateStream = new DeflateStream(output,CompressionMode.Compress,CompressionLevel.BestCompression,false)) {
				deflateStream.Write(data,0,data.Length);
			}
			return output.ToArray();
		}
		public static byte[] DecompressBytes(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			MemoryStream output = new MemoryStream();
			using(var deflateStream = new DeflateStream(input,CompressionMode.Decompress)) {
				deflateStream.CopyTo(output);
			}
			return output.ToArray();
		}*/

		/*if(skinnedMesh!=null) {
			Debug.Log("Writing skeleton");
			writer.Write("skeleton");
			long chunkSize = StringSize(skinnedMesh.rootBone.parent.name)+sizeof(int);
			for(int i = 0;i<skinnedMesh.bones.Length;i++) {
				chunkSize += (long)StringSize(skinnedMesh.bones[i].name)+(sizeof(float)*10)+sizeof(int);
			}
			writer.Write(chunkSize);

			writer.Write(skinnedMesh.rootBone.parent.name);
			writer.Write(skinnedMesh.bones.Length);
			Debug.Log("skeletonName: "+skinnedMesh.rootBone.parent.name);
			Debug.Log("boneCount: "+skinnedMesh.bones.Length);
			for(int i = 0;i<skinnedMesh.bones.Length;i++) {
				Transform bone = skinnedMesh.bones[i];
				writer.Write(bone.name);
				writer.Write(bone.localPosition);
				writer.Write(bone.localRotation);
				writer.Write(bone.localScale);
				if(bone.parent==null) {
					writer.Write(-1);
				}else{
					bool wrote = false;
					for(int j = 0;j<skinnedMesh.bones.Length;j++) {
						if(bone.parent==skinnedMesh.bones[j].parent) {
							writer.Write(j);
							wrote = true;
							break;
						}
					}
					if(!wrote) {
						writer.Write(-1);
					}
				}
			}
		}*/
	}
}
