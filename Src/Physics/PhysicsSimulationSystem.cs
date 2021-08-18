using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Reads(typeof(Rigidbody))]
	public sealed class PhysicsSimulationSystem : GameSystem
	{
		protected internal override void Initialize()
		{
			var physics = WorldPhysics.Default;

			physics.PhysicsWorld = new DiscreteDynamicsWorld(PhysicsEngine.dispatcher, PhysicsEngine.broadphase, null, PhysicsEngine.collisionConf) {
				Gravity = physics.Gravity
			};

			World.Set(physics);
		}

		protected internal override void FixedUpdate()
		{
			if(!World.Has<WorldPhysics>()) {
				return;
			}

			ref var physics = ref World.Get<WorldPhysics>();

			physics.PhysicsWorld.Gravity = physics.Gravity;

			physics.PhysicsWorld.StepSimulation(Time.FixedDeltaTime);
		}
	}
}
