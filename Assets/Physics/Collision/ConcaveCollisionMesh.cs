using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Physics
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
					mesh.vertices[mesh.triangles[i++]],
					mesh.vertices[mesh.triangles[i++]],
					mesh.vertices[mesh.triangles[i++]]
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
