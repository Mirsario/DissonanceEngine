using BulletSharp;

namespace Dissonance.Engine.Physics;

internal static partial class PhysicsInitialization
{
	[WorldSystem, CalledIn<EngineInitialization>]
	static partial void InitializeWorldPhysics(World world)
	{
		var physics = world.Has<WorldPhysics>() ? world.Get<WorldPhysics>() : new();

		physics.CollisionConfiguration ??= new DefaultCollisionConfiguration();
		physics.CollisionDispatcher ??= new CollisionDispatcher(physics.CollisionConfiguration) {
			DispatcherFlags = DispatcherFlags.UseRelativeContactBreakingThreshold,
		};

		physics.Broadphase ??= new DbvtBroadphase();
		//physics.ConstraintSolver ??= new SequentialImpulseConstraintSolver();
		physics.PhysicsWorld ??= new DiscreteDynamicsWorld(physics.CollisionDispatcher, physics.Broadphase, physics.ConstraintSolver, physics.CollisionConfiguration);

		physics.PhysicsWorld.DispatchInfo.EnableSatConvex = false;

		world.Set(physics);
	}

	[MessageSystem, CalledIn<FixedUpdate>, Tags("ReadsMessages")]
	static partial void DisposeWorldPhysics(in ComponentRemovedMessage<WorldPhysics> message)
	{
		ref readonly var physics = ref message.Value;

		physics.PhysicsWorld?.Dispose();
		physics.CollisionDispatcher?.Dispose();
		physics.Broadphase?.Dispose();
		physics.ConstraintSolver?.Dispose();
		physics.CollisionConfiguration?.Dispose();
	}
}
