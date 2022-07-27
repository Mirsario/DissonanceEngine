using BulletSharp;

namespace Dissonance.Engine.Physics;

[Callback<PhysicsUpdateGroup>]
public sealed partial class PhysicsInitializationSystem : GameSystem
{
	[Subsystem]
	partial void InitPhysics()
	{
		var physics = World.Has<WorldPhysics>() ? World.Get<WorldPhysics>() : WorldPhysics.Default;

		physics.CollisionConfiguration ??= new DefaultCollisionConfiguration();
		physics.Broadphase ??= new DbvtBroadphase();
		physics.CollisionDispatcher ??= new CollisionDispatcher(physics.CollisionConfiguration);
		physics.PhysicsWorld ??= new DiscreteDynamicsWorld(physics.CollisionDispatcher, physics.Broadphase, null, physics.CollisionConfiguration) {
			Gravity = physics.Gravity
		};

		World.Set(physics);
	}

	[MessageSubsystem]
	partial void DisposePhysics(in ComponentRemovedMessage<WorldPhysics> message)
	{
		ref readonly var physics = ref message.Value;

		physics.PhysicsWorld?.Dispose();
		physics.CollisionDispatcher?.Dispose();
		physics.Broadphase?.Dispose();
		physics.CollisionConfiguration?.Dispose();
	}
}
