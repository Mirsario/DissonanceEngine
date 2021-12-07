using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Reads<WorldPhysics>]
	[Writes<WorldPhysics>]
	public sealed class PhysicsInitializationSystem : GameSystem
	{
		protected internal override void Initialize()
		{
			var physics = WorldHas<WorldPhysics>() ? WorldGet<WorldPhysics>() : WorldPhysics.Default;

			physics.CollisionDispatcher ??= new CollisionDispatcher(PhysicsEngine.collisionConf);
			physics.PhysicsWorld ??= new DiscreteDynamicsWorld(physics.CollisionDispatcher, PhysicsEngine.broadphase, null, PhysicsEngine.collisionConf) {
				Gravity = physics.Gravity
			};

			WorldSet(physics);
		}
	}
}
