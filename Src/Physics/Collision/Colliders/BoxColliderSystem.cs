﻿using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Callback<ColliderUpdateGroup>]
	public sealed class BoxColliderSystem : GameSystem
	{
		private EntitySet entities;

		protected override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<BoxCollider>() && e.Has<Rigidbody>());
		}

		protected override void Execute()
		{
			// Unregister colliders when their component is removed
			foreach (var message in ReadMessages<ComponentRemovedMessage<BoxCollider>>()) {
				if (message.Value.boxShape != null) {
					SendMessage(new RemoveCollisionShapeMessage(message.Entity, message.Value.boxShape));
					message.Value.boxShape.Dispose();
				}
			}

			foreach (var entity in entities.ReadEntities()) {
				ref var collider = ref entity.Get<BoxCollider>();
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
	}
}
