using System.Collections.Generic;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Reads<WorldPhysics>]
	[Reads<Rigidbody>]
	[Reads<Transform>]
	[Writes<Transform>]
	[Writes<WorldPhysics>]
	public sealed class PhysicsSimulationSystem : GameSystem
	{
		private EntitySet rigidbodyEntities;

		protected internal override void Initialize()
		{
			rigidbodyEntities = World.GetEntitySet(e => e.Has<Rigidbody>() && e.Has<Transform>());

			var physics = WorldHas<WorldPhysics>() ? WorldGet<WorldPhysics>() : WorldPhysics.Default;

			physics.CollisionDispatcher ??= new CollisionDispatcher(PhysicsEngine.collisionConf);
			physics.PhysicsWorld ??= new DiscreteDynamicsWorld(physics.CollisionDispatcher, PhysicsEngine.broadphase, null, PhysicsEngine.collisionConf) {
				Gravity = physics.Gravity
			};

			WorldSet(physics);
		}

		protected internal override void FixedUpdate()
		{
			if (!WorldHas<WorldPhysics>()) {
				return;
			}

			ref var physics = ref WorldGet<WorldPhysics>();

			physics.PhysicsWorld.Gravity = physics.Gravity;

			physics.PhysicsWorld.StepSimulation(Time.FixedDeltaTime);

			// Update collisions

			var collisionDispatcher = physics.CollisionDispatcher;
			int numManifolds = collisionDispatcher.NumManifolds;

			for (int i = 0; i < numManifolds; i++) {
				var contactManifold = collisionDispatcher.GetManifoldByIndexInternal(i);
				int numContacts = contactManifold.NumContacts;

				if (numContacts == 0) {
					continue;
				}

				var objA = contactManifold.Body0;
				var objB = contactManifold.Body1;

				if (objA.UserObject is not Entity entityA || objB.UserObject is not Entity entityB) {
					continue;
				}

				if (!entityA.Has<Rigidbody>() || !entityB.Has<Rigidbody>()) {
					continue;
				}

				ref var rigidbodyA = ref entityA.Get<Rigidbody>();
				ref var rigidbodyB = ref entityB.Get<Rigidbody>();

				for (int j = 0; j < 2; j++) {
					bool processingA = j == 0;
					var otherEntity = processingA ? entityA : entityB;
					ref var thisRigidbody = ref (processingA ? ref rigidbodyA : ref rigidbodyB);

					var contacts = new ContactPoint[numContacts];

					for (int k = 0; k < numContacts; k++) {
						var cPoint = contactManifold.GetContactPoint(k);

						contacts[k] = new ContactPoint(
							processingA ? cPoint.PositionWorldOnB : cPoint.PositionWorldOnA,
							cPoint.NormalWorldOnB,
							cPoint.Distance
						);
					}

					thisRigidbody.collisionsHaveBeenModified = true;
					thisRigidbody.collisions ??= new List<Collision>();

					thisRigidbody.collisions.Add(new Collision(otherEntity, contacts));
				}
			}

			foreach (var entity in rigidbodyEntities.ReadEntities()) {
				ref var rigidbody = ref entity.Get<Rigidbody>();
				ref var transform = ref entity.Get<Transform>();

				// Update transforms based on rigidbody positions
				transform.WorldMatrix = rigidbody.bulletRigidbody.WorldTransform;

				// And also clear collisions if needed
				if (!rigidbody.collisionsHaveBeenModified) {
					rigidbody.collisions?.Clear();
				} else {
					rigidbody.collisionsHaveBeenModified = false;
				}
			}
		}
	}
}
