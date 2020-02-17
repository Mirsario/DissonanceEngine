using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public abstract class CollisionMesh : Asset<CollisionMesh>
	{
		internal CollisionShape collShape;

		public abstract void SetupFromMesh(Mesh mesh);
	}
}
