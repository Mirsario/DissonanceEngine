using BulletSharp;

namespace Dissonance.Engine.Physics;

public struct WorldPhysics
{
	public DbvtBroadphase Broadphase;
	public CollisionConfiguration CollisionConfiguration;
	public DiscreteDynamicsWorld PhysicsWorld;
	public CollisionDispatcher CollisionDispatcher;
	public ConstraintSolver ConstraintSolver;
	public Vector3 Gravity = new(0f, -9.81f, 0f);

	public WorldPhysics() { }
}
