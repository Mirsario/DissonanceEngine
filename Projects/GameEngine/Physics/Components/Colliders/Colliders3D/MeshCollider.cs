using BulletSharp;

namespace GameEngine
{
	public class MeshCollider : Collider
	{
		protected Mesh mesh;
		public Mesh Mesh {
			get => mesh;
			set {
				if(mesh!=value) {
					mesh = value;

					TryUpdateCollider();
				}
			}
		}
		protected bool convex = true;
		public bool Convex {
			get => convex;
			set {
				if(convex!=value) {
					convex = value;

					TryUpdateCollider();
				}
			}
		}

		internal override void UpdateCollider()
		{
			if(collShape!=null) {
				collShape.Dispose();
				collShape = null;
			}

			//TODO:
			if(Mesh!=null) {
				var triMesh = new TriangleMesh();
				for(int i=0;i<Mesh.triangles.Length;i += 3) {
					triMesh.AddTriangle(Mesh.vertices[Mesh.triangles[i]],Mesh.vertices[Mesh.triangles[i+1]],Mesh.vertices[Mesh.triangles[i+2]]);
				}

				if(convex) {
					//Convex shapes can be used for anything
					var tempShape = new ConvexTriangleMeshShape(triMesh);
					using var tempHull = new ShapeHull(tempShape);
					tempHull.BuildHull(tempShape.Margin);
					collShape = new ConvexHullShape(tempHull.Vertices);
				}else{
					//Concave shapes should only be used for static meshes or kinematic rigidbodies
					collShape = new BvhTriangleMeshShape(triMesh,true);
				}
			}

			base.UpdateCollider();
		}
	}
}