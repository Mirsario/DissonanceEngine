using System;
using System.Collections.Generic;
using System.IO;

namespace SmartMeshLibrary
{
	//Not done at all
	public static class SmartMesh
    {
		#region DataStructures
		private struct Vector2
		{
			public float x;
			public float y;

			public Vector2(float x,float y)
			{
				this.x = x;
				this.y = y;
			}
		}
		private struct Vector3
		{
			public float x;
			public float y;
			public float z;

			public Vector3(float x,float y,float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}
		}
		private struct Vector4
		{
			public float x;
			public float y;
			public float z;
			public float w;

			public Vector4(float x,float y,float z,float w)
			{
				this.x = x;
				this.y = y;
				this.z = z;
				this.w = w;
			}
		}
		#endregion

		public const string extension = ".smartmesh";

		public static char[] signature = new char[16] { 'S','m','a','r','t','M','e','s','h',' ','F','o','r','m','a','t' };

		public static SmartMeshFile Import(Stream stream)
		{
			SmartMeshChunk[] chunks;
			using(var reader = new BinaryReader(stream)) {
				if(string.CompareOrdinal(new string(reader.ReadChars(signature.Length)),new string(signature))!=0) {
					throw new NotSupportedException("Specified stream is not a SmartMesh file");
				}
				chunks = SmartMeshChunk.ReadChunks(reader);
			}

			var meshFile = new SmartMeshFile(chunks);
			var objectChunks = meshFile.GetChunks("object");
			var objects = new List<SmartMeshObject>();
			foreach(var chunk in objectChunks) {
				using(var r = chunk.GetReader()) {
					objects.Add(SmartMeshObject.FromChunk(r));
				}
			}
			meshFile.objects = objects;
			return meshFile;

			/*Mesh mesh = new Mesh {
				name = fileName ?? "UntitledMesh",
				vertices = vertices,
				triangles = triangles,
				uv = uv0,
				normals = normals,
				tangents = tangents,
				colors = colors,
				boneWeights = boneWeights,
				skeleton = skeleton
			};*/
			//if(normals==null) {
			//	mesh.RecalculateNormals();
			//}
			//mesh.Apply();
			//return mesh;
		}

		/*public override void Export(Mesh mesh,Stream stream)
        {
			var writer = new BinaryWriter(stream);

			int chunkAmount = 0;
			long amountPos = writer.BaseStream.Position;
			writer.Write(0);	//32 bit int to be rewritten at the end
			writer.Write(mesh.vertices.Length);
			writer.Write(mesh.triangles.Length);

			#region Vertices
			WriteChunk(writer,"vertices",w => {
				for(int i=0;i<mesh.vertexCount;i++) {
					w.Write(mesh.vertices[i]);
				}
				chunkAmount++;
			});
			#endregion
			#region Triangles
			WriteChunk(writer,"triangles",w => {
				for(int i=0;i<mesh.triangles.Length;i++) {
					w.Write(mesh.triangles[i]);
				}
				chunkAmount++;
			});
			#endregion
			#region UV
			if(mesh.uv?.Length>0==true) {
				WriteChunk(writer,"uv0",w => {
					for(int i=0;i<mesh.vertexCount;i++) {
						w.Write(mesh.uv[i]);
					}
				});
				chunkAmount++;
			}
			#endregion
			#region Normals
			if(mesh.normals?.Length>0==true) {
				WriteChunk(writer,"normals",w => {
					for(int i=0;i<mesh.vertexCount;i++) {
						w.Write(mesh.normals[i]);
					}
				});
				chunkAmount++;
			}
			#endregion
			#region Tangents
			if(mesh.tangents?.Length>0==true) {
				WriteChunk(writer,"tangents",w => {
					for(int i=0;i<mesh.vertexCount;i++) {
						w.Write(mesh.tangents[i]);
					}
				});
				chunkAmount++;
			}
			#endregion
			#region VertexColor
			if(mesh.colors?.Length>0==true) {
				WriteChunk(writer,"colors",w => {
					for(int i=0;i<mesh.vertexCount;i++) {
						w.Write((byte)(mesh.colors[i].x*255));
						w.Write((byte)(mesh.colors[i].y*255));
						w.Write((byte)(mesh.colors[i].z*255));
						w.Write((byte)(mesh.colors[i].w*255));
					}
				});
				chunkAmount++;
			}
			#endregion
			#region BoneWeights
			if(mesh.boneWeights?.Length>0==true) {
				WriteChunk(writer,"boneWeights",w => {
					for(int i=0;i<mesh.vertexCount;i++) {
						w.Write(mesh.boneWeights[i].boneIndex0);
						w.Write(mesh.boneWeights[i].boneIndex1);
						w.Write(mesh.boneWeights[i].boneIndex2);
						w.Write(mesh.boneWeights[i].boneIndex3);
						w.Write(mesh.boneWeights[i].weight0);
						w.Write(mesh.boneWeights[i].weight1);
						w.Write(mesh.boneWeights[i].weight2);
						w.Write(mesh.boneWeights[i].weight3);
					}
				});
				chunkAmount++;
			}
			#endregion
			long prevPos = writer.BaseStream.Position;
			writer.BaseStream.Position = amountPos;
			writer.Write(chunkAmount);
			writer.BaseStream.Position = prevPos;
			writer.Dispose();
		}*/

		/*public static void WriteChunk(BinaryWriter writer,string name,Action<BinaryWriter> action)
		{
			writer.Write((byte)name.Length);
			writer.Write(name.ToCharArray());
			MemoryStream output = new MemoryStream();
			using(var deflateStream = new DeflateStream(output,CompressionMode.Compress,CompressionLevel.BestCompression,false)) {
				using(var newWriter = new BinaryWriter(deflateStream)) {
					action(newWriter);
				}
			}
			byte[] data = output.ToArray();
			writer.Write((ulong)data.LongLength);
			writer.Write(data);
		}
		public static void ReadChunk(BinaryReader reader,Action<string,BinaryReader> action)
		{
			string chunkName = new string(reader.ReadChars(reader.ReadByte()));//reader.ReadString();//
			ulong length = reader.ReadUInt64();
			MemoryStream input = new MemoryStream(reader.ReadBytes((int)length));//Shouldn't there be an overload which'd take a long?
			MemoryStream output = new MemoryStream();
			using(var stream = new DeflateStream(input,CompressionMode.Decompress)) {
				stream.CopyTo(output);
			}
			output.Position = 0;
			using(var newReader = new BinaryReader(output)) {
				action(chunkName,newReader);
			}
			output?.Dispose();
		}*/
	}
}
