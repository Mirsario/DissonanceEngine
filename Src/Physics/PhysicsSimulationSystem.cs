using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Reads(typeof(WorldPhysics), typeof(Rigidbody))]
	[Writes(typeof(Transform))]
	public sealed class PhysicsSimulationSystem : GameSystem
	{
		private EntitySet rigidbodyEntities;

		protected internal override void Initialize()
		{
			rigidbodyEntities = World.GetEntitySet(e => e.Has<Rigidbody>() && e.Has<Transform>());

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

			// Update transforms based on rigidbody positions

			foreach(var entity in rigidbodyEntities.ReadEntities()) {
				ref var rigidbody = ref entity.Get<Rigidbody>();
				ref var transform = ref entity.Get<Transform>();

				transform.WorldMatrix = rigidbody.bulletRigidbody.WorldTransform;
			}
		}
	}
}
