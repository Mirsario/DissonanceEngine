namespace Dissonance.Engine.Physics
{
	[Reads(typeof(MeshCollider))]
	[Writes(typeof(MeshCollider))]
	[Receives(typeof(ComponentRemovedMessage<MeshCollider>))]
	[Sends(typeof(AddCollisionShapeMessage), typeof(RemoveCollisionShapeMessage))]
	public sealed class MeshColliderSystem : GameSystem
	{
		private EntitySet entities;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<MeshCollider>());
		}

		protected internal override void FixedUpdate()
		{
			// Unregister colliders when their component is removed
			foreach (var message in ReadMessages<ComponentRemovedMessage<MeshCollider>>()) {
				var collisionShape = message.Value.CollisionMesh?.collisionShape;

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

					var collisionShape = collider.CollisionMesh?.collisionShape;

					if (collisionShape != null) {
						SendMessage(new AddCollisionShapeMessage(entity, collisionShape));
					}

					collider.needsUpdate = false;
				}
			}
		}
	}
}
