namespace Dissonance.Engine.Physics;

[Callback<ColliderUpdateGroup>]
public sealed partial class MeshColliderSystem : GameSystem
{
	[MessageSubsystem]
	partial void DisposeColliders(in ComponentRemovedMessage<MeshCollider> message, World world)
	{
		// Unregister colliders when their component is removed
		var collisionShape = message.Value.CollisionMesh?.CollisionShape;

		if (collisionShape != null) {
			world.SendMessage(new RemoveCollisionShapeMessage(message.Entity, collisionShape));
		}
	}

	[EntitySubsystem]
	partial void UpdateColliders(World world, Entity entity, ref MeshCollider collider)
	{
		if (collider.needsUpdate) {
			if (collider.lastCollisionShape != null) {
				world.SendMessage(new RemoveCollisionShapeMessage(entity, collider.lastCollisionShape));
			}

			var collisionShape = collider.CollisionMesh?.CollisionShape;

			if (collisionShape != null) {
				world.SendMessage(new AddCollisionShapeMessage(entity, collisionShape));
			}

			collider.lastCollisionShape = collisionShape;
			collider.needsUpdate = false;
		}
	}
}
