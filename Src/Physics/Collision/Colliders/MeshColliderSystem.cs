namespace Dissonance.Engine.Physics
{
	[Callback<ColliderUpdateGroup>]
	public sealed class MeshColliderSystem : GameSystem
	{
		private EntitySet entities;

		protected override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<MeshCollider>());
		}

		protected override void Execute()
		{
			// Unregister colliders when their component is removed
			foreach (var message in ReadMessages<ComponentRemovedMessage<MeshCollider>>()) {
				var collisionShape = message.Value.CollisionMesh?.CollisionShape;

				if (collisionShape != null) {
					SendMessage(new RemoveCollisionShapeMessage(message.Entity, collisionShape));
				}
			}

			foreach (var entity in entities.ReadEntities()) {
				ref var collider = ref entity.Get<MeshCollider>();

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
}
