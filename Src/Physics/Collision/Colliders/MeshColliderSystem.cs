namespace Dissonance.Engine.Physics
{
	[Reads<MeshCollider>]
	[Writes<MeshCollider>]
	[Receives<ComponentRemovedMessage<MeshCollider>>]
	[Sends<AddCollisionShapeMessage>]
	[Sends<RemoveCollisionShapeMessage>]
	public sealed partial class MeshColliderSystem : GameSystem
	{
		[MessageSubsystem]
		private partial void DisposeColliders(in ComponentRemovedMessage<MeshCollider> message)
		{
			// Unregister colliders when their component is removed
			var collisionShape = message.Value.CollisionMesh?.CollisionShape;

			if (collisionShape != null) {
				SendMessage(new RemoveCollisionShapeMessage(message.Entity, collisionShape));
			}
		}

		[EntitySubsystem]
		private partial void UpdateColliders(Entity entity, ref MeshCollider collider)
		{
			if (collider.needsUpdate) {
				if (collider.lastCollisionShape != null) {
					SendMessage(new RemoveCollisionShapeMessage(entity, collider.lastCollisionShape));
				}

				var collisionShape = collider.CollisionMesh?.CollisionShape;

				if (collisionShape != null) {
					SendMessage(new AddCollisionShapeMessage(entity, collisionShape));
				}

				collider.needsUpdate = false;
			}
		}
	}
}
