using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Callback<PhysicsUpdateGroup>]
	public sealed class PhysicsInitializationSystem : GameSystem
	{
		protected override void Initialize()
		{
			var physics = World.Has<WorldPhysics>() ? World.Get<WorldPhysics>() : WorldPhysics.Default;

			physics.CollisionDispatcher ??= new CollisionDispatcher(PhysicsEngine.collisionConf);
			physics.PhysicsWorld ??= new DiscreteDynamicsWorld(physics.CollisionDispatcher, PhysicsEngine.broadphase, null, PhysicsEngine.collisionConf) {
				Gravity = physics.Gravity
			};

			World.Set(physics);
		}
	}
}
