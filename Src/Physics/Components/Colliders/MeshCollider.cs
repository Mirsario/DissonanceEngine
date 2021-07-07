using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public struct MeshCollider
	{
		internal bool needsUpdate;
		internal CollisionShape lastCollisionShape;
		
		private CollisionMesh collisionMesh;

		public CollisionMesh CollisionMesh {
			get => collisionMesh;
			set {
				if(value != collisionMesh) {
					collisionMesh = value;
					needsUpdate = true;
				}
			}
		}

		public MeshCollider(CollisionMesh collisionMesh) : this()
		{
			CollisionMesh = collisionMesh;
		}
	}
}
