using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Reads(typeof(CapsuleCollider))]
	[Writes(typeof(CapsuleCollider))]
	[Receives(typeof(ComponentRemovedMessage<CapsuleCollider>))]
	[Sends(typeof(AddCollisionShapeMessage), typeof(RemoveCollisionShapeMessage))]
	public sealed class CapsuleColliderSystem : GameSystem
	{
		private EntitySet entities;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<CapsuleCollider>());
		}

		protected internal override void FixedUpdate()
		{
			// Unregister colliders when their component is removed
			foreach(var message in ReadMessages<ComponentRemovedMessage<CapsuleCollider>>()) {
				if(message.Value.capsuleShape != null) {
					SendMessage(new RemoveCollisionShapeMessage(message.Entity, message.Value.capsuleShape));
					message.Value.capsuleShape.Dispose();
				}
			}

			foreach(var entity in entities.ReadEntities()) {
				ref var collider = ref entity.Get<CapsuleCollider>();
				bool noShape = collider.capsuleShape == null;

				if(noShape || collider.needsUpdate) {
					if(!noShape) {
						SendMessage(new RemoveCollisionShapeMessage(entity, collider.capsuleShape));
						collider.capsuleShape.Dispose();
					}

					collider.capsuleShape = new CapsuleShape(collider.Radius, collider.Height);
					collider.needsUpdate = false;

					SendMessage(new AddCollisionShapeMessage(entity, collider.capsuleShape));
				}
			}
		}
	}
}
