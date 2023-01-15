using BulletSharp;

namespace Dissonance.Engine.Physics;

[Callback<PhysicsUpdateGroup>]
public sealed partial class PhysicsInitializationSystem : GameSystem
{
	[WorldSubsystem]
	partial void InitPhysics(World world)
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

	[MessageSubsystem]
	partial void DisposePhysics(in ComponentRemovedMessage<WorldPhysics> message)
	{
		ref readonly var physics = ref message.Value;

		physics.PhysicsWorld?.Dispose();
		physics.CollisionDispatcher?.Dispose();
		physics.Broadphase?.Dispose();
		physics.ConstraintSolver?.Dispose();
		physics.CollisionConfiguration?.Dispose();
	}
}
