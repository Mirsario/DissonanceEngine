using BulletSharp;

namespace Dissonance.Engine.Physics
{
	//Concave shapes should only be used for static meshes or kinematic rigidbodies
	public class ConcaveCollisionMesh : CollisionMesh
	{
		public override void SetupFromMesh(Mesh mesh)
		{
			var triMesh = new TriangleMesh();

			int i = 0;

			var vertices = mesh.Vertices;

			while(i<mesh.Indices.Length) {
				triMesh.AddTriangle(
					vertices[mesh.Indices[i++]],
					vertices[mesh.Indices[i++]],
					vertices[mesh.Indices[i++]]
				);
			}

			collShape = new BvhTriangleMeshShape(triMesh,true);
		}

		public static explicit operator ConcaveCollisionMesh(Mesh mesh)
		{
			var collisionMesh = new ConcaveCollisionMesh();

			collisionMesh.SetupFromMesh(mesh);

			return collisionMesh;
		}
	}
}
