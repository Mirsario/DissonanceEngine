using System.Collections.Generic;

namespace Dissonance.Engine
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
			mesh.Vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();

			if(normals!=null && normals.Count>0) {
				mesh.Normals = normals.ToArray();
			}

			if(uvs!=null && uvs.Count>0) {
				mesh.Uv0 = uvs.ToArray();
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
