/*using BulletSharp;
using Dissonance.Engine.Utils.Extensions;

namespace Dissonance.Engine.Physics
{
	//Concave shapes should only be used for static meshes or kinematic rigidbodies
	public class ConcaveCollisionMesh : CollisionMesh
	{
		public override void SetupFromMesh(Mesh mesh)
		{
			var triMesh = new TriangleMesh();

			int i = 0;

			while(i<mesh.triangles.Length) {
				triMesh.AddTriangle(
					mesh.vertices[mesh.triangles[i++]].ToBulletVector3(),
					mesh.vertices[mesh.triangles[i++]].ToBulletVector3(),
					mesh.vertices[mesh.triangles[i++]].ToBulletVector3()
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
}*/
