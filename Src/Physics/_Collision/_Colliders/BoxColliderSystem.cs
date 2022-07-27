using BulletSharp;

namespace Dissonance.Engine.Physics;

[Callback<ColliderUpdateGroup>]
public sealed partial class BoxColliderSystem : GameSystem
{
	[MessageSubsystem]
	partial void DisposeColliders(in ComponentRemovedMessage<BoxCollider> message)
	{
		// Unregister colliders when their component is removed
		if (message.Value.boxShape != null) {
			SendMessage(new RemoveCollisionShapeMessage(message.Entity, message.Value.boxShape));
			message.Value.boxShape.Dispose();
		}
	}

	[EntitySubsystem]
	partial void Update(Entity entity, ref BoxCollider collider)
	{
		bool noShape = collider.boxShape == null;

		if (noShape || collider.needsUpdate) {
			if (!noShape) {
				SendMessage(new RemoveCollisionShapeMessage(entity, collider.boxShape));

				collider.boxShape.Dispose();
			}

			collider.boxShape = new BoxShape(collider.Size * 0.5f);
			collider.needsUpdate = false;

			SendMessage(new AddCollisionShapeMessage(entity, collider.boxShape));
		}
	}
}
