using BulletSharp;

namespace Dissonance.Engine.Physics.Systems
{
	public sealed class PhysicsSimulationSystem : GameSystem
	{
		public override void Initialize()
		{
			var physics = WorldPhysics.Default;

			physics.PhysicsWorld = new DiscreteDynamicsWorld(PhysicsEngine.dispatcher, PhysicsEngine.broadphase, null, PhysicsEngine.collisionConf);

			World.Set(physics);
		}

		public override void FixedUpdate()
		{
			if(!World.Has<WorldPhysics>()) {
				return;
			}

			ref var physics = ref World.Get<WorldPhysics>();

			physics.PhysicsWorld.StepSimulation(Time.FixedDeltaTime);
		}
	}
}
