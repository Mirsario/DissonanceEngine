using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Physics
{
	//Convex shapes can be used for anything
	public class ConvexCollisionMesh : CollisionMesh
	{
		public override void SetupFromMesh(Mesh mesh)
		{
			var triMesh = new TriangleMesh();
			for(int i = 0;i<mesh.triangles.Length;i += 3) {
				triMesh.AddTriangle(mesh.vertices[mesh.triangles[i]],mesh.vertices[mesh.triangles[i+1]],mesh.vertices[mesh.triangles[i+2]]);
			}

			var tempShape = new ConvexTriangleMeshShape(triMesh);
			using var tempHull = new ShapeHull(tempShape);

			tempHull.BuildHull(tempShape.Margin);

			collShape = new ConvexHullShape(tempHull.Vertices);
		}
	}
}
