namespace Dissonance.Engine.Graphics
{
	public class NormalBuffer : CustomVertexBuffer<Vector3>
	{
		public void Recalculate()
		{
			var vertices = mesh.Vertices;
			var triangles = mesh.Triangles;

			var newNormals = new Vector3[vertices.Length];

			for(uint i = 0;i<triangles.Length;i+=3) {
				uint i1 = triangles[i];
				uint i2 = triangles[i+1];
				uint i3 = triangles[i+2];

				var v1 = vertices[i1];
				var v2 = vertices[i2];
				var v3 = vertices[i3];

				var normal = Vector3.Cross(v2-v1,v3-v1).Normalized;

				newNormals[i1] += normal;
				newNormals[i2] += normal;
				newNormals[i3] += normal;
			}

			var zero = Vector3.Zero;

			for(int i = 0;i<vertices.Length;i++) {
				if(newNormals[i]!=zero) {
					newNormals[i].Normalize();
				}
			}

			data = newNormals;
		}
	}
}
