namespace Dissonance.Engine.Physics;

internal static partial class MeshColliderImplementation
{
	[MessageSystem, CalledIn<PhysicsUpdate>, Tags("ColliderUpdate")]
	static partial void DisposeMeshColliders(in ComponentRemovedMessage<MeshCollider> message, World world)
	{
		// Unregister colliders when their component is removed
		var collisionShape = message.Value.CollisionMesh?.CollisionShape;

		if (collisionShape != null) {
			world.SendMessage(new RemoveCollisionShapeMessage(message.Entity, collisionShape));
		}
	}

	[EntitySystem, CalledIn<PhysicsUpdate>, Tags("ColliderUpdate")]
	static partial void UpdateMeshColliders(World world, Entity entity, ref MeshCollider collider)
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
