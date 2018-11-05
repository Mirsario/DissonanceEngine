using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartMeshLibrary
{
	public enum IntType
	{
		UInt8,
		UInt16,
		Int32 //Should this be unsigned???
	}
	public class SmartMeshObject : ChunkContainer
	{
		public string name;
		
		public SmartMeshObject(string name,IEnumerable<SmartMeshChunk> chunksToAdd = null) : base(chunksToAdd)
		{
			this.name = name;
		}

		#region BuiltInChunks
		#region Vertices
		public TVector3[] ReadVertices<TVector3>(NewVector3<TVector3> newVector3) => TryReadChunk("vertices",reader => SmartMeshUtils.ReadVector3Array(reader,newVector3));
		public void WriteVertices<TVector3>(TVector3[] array,Vector3ToXYZ<TVector3> vector3ToXYZ) => TryWriteChunk("vertices",writer => SmartMeshUtils.WriteVector3Array(writer,array,vector3ToXYZ));
		#endregion
		#region Triangles
		public int[] ReadTriangles() => TryReadChunk("triangles",SmartMeshUtils.ReadDynamicIntArray);
		public void WriteTriangles(int[] array) => TryWriteChunk("triangles",writer => SmartMeshUtils.WriteDynamicIntArray(writer,array));
		#endregion
		#region Normals
		public TVector3[] ReadNormals<TVector3>(NewVector3<TVector3> newVector3) => TryReadChunk("normals",reader => SmartMeshUtils.ReadVector3Array(reader,newVector3));
		public void WriteNormals<TVector3>(TVector3[] array,Vector3ToXYZ<TVector3> vector3ToXYZ) => TryWriteChunk("normals",writer => SmartMeshUtils.WriteVector3Array(writer,array,vector3ToXYZ));
		#endregion
		#endregion

		public static SmartMeshObject FromChunk(BinaryReader reader) => new SmartMeshObject(reader.ReadString(),SmartMeshChunk.ReadChunks(reader));
	}
}
