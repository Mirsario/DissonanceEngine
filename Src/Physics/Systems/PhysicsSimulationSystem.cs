using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public sealed class PhysicsSimulationSystem : GameSystem
	{
		protected internal override void Initialize()
		{
			var physics = WorldPhysics.Default;

			physics.PhysicsWorld = new DiscreteDynamicsWorld(PhysicsEngine.dispatcher, PhysicsEngine.broadphase, null, PhysicsEngine.collisionConf) {
				Gravity = new Vector3(0f, -9.807f, 0f)
			};

			World.Set(physics);
		}

		protected internal override void FixedUpdate()
		{
			if(!World.Has<WorldPhysics>()) {
				return;
			}

			ref var physics = ref World.Get<WorldPhysics>();

			physics.PhysicsWorld.StepSimulation(Time.FixedDeltaTime);
		}
	}
}
