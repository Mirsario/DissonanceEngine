using System.Collections.Generic;

namespace Dissonance.Engine.Physics;

public static partial class PhysicsSimulation
{
	[WorldSystem, CalledIn<PhysicsUpdate>, Tags("PhysicsSimulation"), RequiresTags("ColliderUpdate", "RigidbodyUpdate")]
	static partial void StepSimulation([FromWorld] ref WorldPhysics physics)
	{
		physics.PhysicsWorld.Gravity = physics.Gravity;

		physics.PhysicsWorld.StepSimulation(Time.FixedDeltaTime);
	}

	[EntitySystem, CalledIn<PhysicsUpdate>, Tags("RigidbodyCollisionsClearing"), RequiresTags("PhysicsSimulation")]
	static partial void ClearRigidbodyCollisions(ref Rigidbody rigidbody)
	{
		rigidbody.collisions?.Clear(); //TODO: Optimize, avoid reallocations every frame...
	}
	
	[WorldSystem, CalledIn<PhysicsUpdate>, Tags("RigidbodyCollisions"), RequiresTags("PhysicsSimulation", "RigidbodyCollisionsClearing")]
	static partial void UpdateRigidbodyCollisions([FromWorld] ref WorldPhysics physics)
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

				thisRigidbody.collisions ??= new List<Collision>();

				thisRigidbody.collisions.Add(new Collision(otherEntity, contacts));
			}
		}
	}
	
	[EntitySystem, CalledIn<PhysicsUpdate>, RequiresTags("PhysicsSimulation", "RigidbodyUpdate")]
	static partial void UpdateRigidbodyTransformations(ref Rigidbody rigidbody, ref Transform transform)
	{
		transform.WorldMatrix = rigidbody.bulletRigidbody.WorldTransform;
	}
}
