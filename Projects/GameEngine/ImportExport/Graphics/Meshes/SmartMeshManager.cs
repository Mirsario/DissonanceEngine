using System.IO;
using System.Collections.Generic;
using SmartMeshLibrary;

// ReSharper disable UnusedMember.Local

namespace GameEngine
{
	public class SmartMeshManager : AssetManager<Mesh>
	{
		public override string[] Extensions => new[] { ".smartmesh" };
		
		public override Mesh Import(Stream stream,string fileName) //TODO: Bad design choices, read AssetManager.cs
		{
			var file = SmartMesh.Import(stream);
			var meshes = new List<Mesh>();
			for(int i = 0;i<file.objects.Count;i++) {
				var obj = file.objects[i];

				var mesh = new Mesh {
					vertices = obj.ReadVertices(NewVector3),
					triangles = obj.ReadTriangles(),
					normals = obj.ReadNormals(NewVector3),
				};

				meshes.Add(mesh);
			}
			return meshes.Count>0 ? meshes[0] : null;
		}
		public override void Export(Mesh mesh,Stream stream) //TODO: Bad design choices, read AssetManager.cs
		{
			using var file = new SmartMeshFile();

			var obj = new SmartMeshObject(mesh.name);

			obj.WriteVertices(mesh.vertices,Vector3ToXYZ);
			obj.WriteTriangles(mesh.triangles);
			obj.WriteNormals(mesh.normals,Vector3ToXYZ);

			file.objects.Add(obj);

			//file.Export(stream);
		}

		private static Vector2 NewVector2(float x,float y) => new Vector2(x,y);
		private static Vector3 NewVector3(float x,float y,float z) => new Vector3(x,y,z);
		private static Vector4 NewVector4(float x,float y,float z,float w) => new Vector4(x,y,z,w);
		private static (float x,float y) Vector2ToXY(Vector2 vec) => (vec.x,vec.y);
		private static (float x,float y,float z) Vector3ToXYZ(Vector3 vec) => (vec.x,vec.y,vec.z);
		private static (float x,float y,float z,float w) Vector4ToXYZW(Vector4 vec) => (vec.x,vec.y,vec.z,vec.w);
	}
}
