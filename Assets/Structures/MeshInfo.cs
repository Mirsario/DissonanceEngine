using System.Collections.Generic;

namespace GameEngine
{
	public class MeshInfo
	{
		public List<Vector3> vertices;
		public List<Vector3> normals;	
		public List<int> triangles;
		public List<Vector2> uvs;
		
		public MeshInfo(bool hasNormals = true,bool hasUVs = true)
		{
			vertices = new List<Vector3>();
			triangles = new List<int>();
			normals = hasNormals ? new List<Vector3>() : null;
			uvs = hasUVs ? new List<Vector2>() : null;
		}
		public void ApplyToMesh(Mesh mesh,bool clear = true)
		{
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();

			if(normals!=null && normals.Count>0) {
				mesh.normals = normals.ToArray();
			}

			if(uvs!=null && uvs.Count>0) {
				mesh.uv = uvs.ToArray();
			}

			if(clear) {
				vertices.Clear();
				triangles.Clear();
				normals?.Clear();
				uvs?.Clear();
			}
		}
	}
}
