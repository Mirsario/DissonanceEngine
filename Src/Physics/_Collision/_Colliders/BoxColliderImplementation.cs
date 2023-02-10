using BulletSharp;

namespace Dissonance.Engine.Physics;

internal static partial class BoxColliderImplementation
{
	[MessageSystem, CalledIn<PhysicsUpdate>, Tags("ColliderUpdate")]
	static partial void DisposeBoxColliders(in ComponentRemovedMessage<BoxCollider> message, World world)
	{
		// Unregister colliders when their component is removed
		if (message.Value.boxShape != null) {
			world.SendMessage(new RemoveCollisionShapeMessage(message.Entity, message.Value.boxShape));
			message.Value.boxShape.Dispose();
		}
	}

	[EntitySystem, CalledIn<PhysicsUpdate>, Tags("ColliderUpdate")]
	static partial void UpdateBoxColliders(World world, Entity entity, ref BoxCollider collider)
	{
		bool noShape = collider.boxShape == null;

		if (noShape || collider.needsUpdate) {
			if (!noShape) {
				world.SendMessage(new RemoveCollisionShapeMessage(entity, collider.boxShape));

				collider.boxShape.Dispose();
			}

			collider.boxShape = new BoxShape(collider.Size * 0.5f);
			collider.needsUpdate = false;

			world.SendMessage(new AddCollisionShapeMessage(entity, collider.boxShape));
		}
	}
}
