using System.Collections.Generic;

namespace Dissonance.Engine.Physics
{
	[Reads<WorldPhysics>]
	[Reads<Rigidbody>]
	[Reads<Transform>]
	[Writes<Transform>]
	public sealed partial class PhysicsSimulationSystem : GameSystem
	{
		[Subsystem]
		private static partial void StepSimulation([FromWorld] ref WorldPhysics physics)
		{
			physics.PhysicsWorld.Gravity = physics.Gravity;

			physics.PhysicsWorld.StepSimulation(Time.FixedDeltaTime);
		}

		[Subsystem]
		private static partial void UpdateCollisions([FromWorld] ref WorldPhysics physics)
		{
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
		}

		[EntitySubsystem]
		private static partial void UpdateRigidbodies(ref Rigidbody rigidbody, ref Transform transform)
		{
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
