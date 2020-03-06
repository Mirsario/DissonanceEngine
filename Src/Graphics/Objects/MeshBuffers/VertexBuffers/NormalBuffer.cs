using System;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class NormalBuffer : CustomVertexBuffer<Vector3>
	{
		public void Recalculate()
		{
			if(mesh.PrimitiveType!=PrimitiveType.Triangles) {
				throw new InvalidOperationException($"{nameof(Recalculate)}() can only work on meshes with {nameof(PrimitiveType.Triangles)} {nameof(Mesh.PrimitiveType)}.");
			}

			var vertices = mesh.Vertices ?? throw new InvalidOperationException($"{nameof(Recalculate)}() requires {nameof(VertexBuffer)} to be ready.");
			var triangles = mesh.Triangles;
			bool isIndexed = triangles!=null;
			int indexCount = isIndexed ? triangles.Length : vertices.Length;

			if(!isIndexed && indexCount%3!=0) {
				throw new InvalidOperationException($"{nameof(Recalculate)}() requires vertex count to be divisable by 3, if no indices are provided.");
			}

			var newNormals = new Vector3[vertices.Length];

			for(uint i = 0;i<indexCount;i+=3) {
				uint i1,i2,i3;

				if(isIndexed) {
					i1 = triangles[i];
					i2 = triangles[i+1];
					i3 = triangles[i+2];
				} else {
					i1 = i;
					i2 = i+1;
					i3 = i+2;
				}

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
