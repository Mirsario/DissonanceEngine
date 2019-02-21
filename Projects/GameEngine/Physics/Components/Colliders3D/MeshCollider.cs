using BulletSharp;

namespace GameEngine
{
	public class MeshCollider : Collider
	{
		private Mesh _mesh;
		public Mesh Mesh {
			get => _mesh;
			set {
				if(_mesh!=value) {
					_mesh = value;
					UpdateCollider();
				}
				/*if(!Physics.collidersToUpdate.Contains(this)) {
					Physics.collidersToUpdate.Add(this);
				}*/
			}
		}
		private bool _convex = true;
		public bool Convex {
			get => _convex;
			set {
				if(_convex==value) {
					return;
				}
				_convex = value;
				if(Mesh!=null) {
					UpdateCollider();
				}
			}
		}

		protected override void OnEnable()
		{
			/*if(!Physics.collidersToUpdate.Contains(this)) {
				Physics.collidersToUpdate.Add(this);
			}*/
		}
		protected override void OnDisable()
		{
			
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
				if(_convex) {
					//Convex shapes can be used for anything
					var tempShape = new ConvexTriangleMeshShape(triMesh);
					var tempHull = new ShapeHull(tempShape);
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
/*//Convert file data to TriangleMesh
var trimesh = new TriangleMesh();
trimeshes.Add(trimesh);

Vector3 localScaling = new Vector3(6,6,6);
List<int> indices = wo.Indices;
List<Vector3> vertices = wo.Vertices;

int i;
for (i = 0; i < tcount; i++)
{
    int index0 = indices[i*3];
    int index1 = indices[i*3+1];
    int index2 = indices[i*3+2];

    Vector3 vertex0 = vertices[index0]*localScaling;
    Vector3 vertex1 = vertices[index1]*localScaling;
    Vector3 vertex2 = vertices[index2]*localScaling;

    trimesh.AddTriangleRef(ref vertex0,ref vertex1,ref vertex2);
}

//Create a hull approximation
ConvexHullShape convexShape;
using (var tmpConvexShape = new ConvexTriangleMeshShape(trimesh))
{
    using (var hull = new ShapeHull(tmpConvexShape))
    {
        hull.BuildHull(tmpConvexShape.Margin);
        convexShape = new ConvexHullShape(hull.Vertices);
    }
}
if (sEnableSAT)
{
    convexShape.InitializePolyhedralFeatures();
}
CollisionShapes.Add(convexShape);*/