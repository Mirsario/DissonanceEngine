using BulletSharp;

namespace Dissonance.Engine.Physics;

internal static partial class CapsuleColliderImplementation
{
	[MessageSystem, CalledIn<PhysicsUpdate>, Tags("ColliderUpdate")]
	static partial void DisposeCapsuleColliders(in ComponentRemovedMessage<CapsuleCollider> message, World world)
	{
		// Unregister colliders when their component is removed
		if (message.Value.capsuleShape != null) {
			world.SendMessage(new RemoveCollisionShapeMessage(message.Entity, message.Value.capsuleShape));
			message.Value.capsuleShape.Dispose();
		}
	}

	[EntitySystem, CalledIn<PhysicsUpdate>, Tags("ColliderUpdate")]
	static partial void UpdateCapsuleColliders(World world, Entity entity, ref CapsuleCollider collider)
	{
		bool noShape = collider.capsuleShape == null;

		if (noShape || collider.needsUpdate) {
			if (!noShape) {
				world.SendMessage(new RemoveCollisionShapeMessage(entity, collider.capsuleShape));
				collider.capsuleShape.Dispose();
			}

			collider.capsuleShape = new CapsuleShape(collider.Radius * 0.5f, collider.Height * 0.5f);
			collider.needsUpdate = false;

			world.SendMessage(new AddCollisionShapeMessage(entity, collider.capsuleShape));
		}
	}
}
