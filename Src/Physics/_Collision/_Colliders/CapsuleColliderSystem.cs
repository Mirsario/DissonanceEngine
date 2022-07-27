using BulletSharp;

namespace Dissonance.Engine.Physics;

[Callback<ColliderUpdateGroup>]
public sealed partial class CapsuleColliderSystem : GameSystem
{
	[MessageSubsystem]
	partial void DisposeColliders(in ComponentRemovedMessage<CapsuleCollider> message)
	{
		// Unregister colliders when their component is removed
		if (message.Value.capsuleShape != null) {
			SendMessage(new RemoveCollisionShapeMessage(message.Entity, message.Value.capsuleShape));
			message.Value.capsuleShape.Dispose();
		}
	}

	[EntitySubsystem]
	partial void Update(Entity entity, ref CapsuleCollider collider)
	{
		bool noShape = collider.capsuleShape == null;

		if (noShape || collider.needsUpdate) {
			if (!noShape) {
				SendMessage(new RemoveCollisionShapeMessage(entity, collider.capsuleShape));
				collider.capsuleShape.Dispose();
			}

			collider.capsuleShape = new CapsuleShape(collider.Radius, collider.Height);
			collider.needsUpdate = false;

			SendMessage(new AddCollisionShapeMessage(entity, collider.capsuleShape));
		}
	}
}
