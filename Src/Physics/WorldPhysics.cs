using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public struct WorldPhysics
	{
		public static readonly WorldPhysics Default = new() {
			Gravity = new Vector3(0f, -9.81f, 0f)
		};

		public DbvtBroadphase Broadphase;
		public CollisionConfiguration CollisionConfiguration;
		public DiscreteDynamicsWorld PhysicsWorld;
		public CollisionDispatcher CollisionDispatcher;
		public Vector3 Gravity;
	}
}
