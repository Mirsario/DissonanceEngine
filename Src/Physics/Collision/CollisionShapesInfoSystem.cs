using System.Collections.Generic;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Reads<CollisionShapesInfo>]
	[Writes<CollisionShapesInfo>]
	[Receives<AddCollisionShapeMessage>]
	[Receives<RemoveCollisionShapeMessage>]
	public sealed class CollisionShapesInfoSystem : GameSystem
	{
		protected internal override void FixedUpdate()
		{
			// Remove collision shapes
			foreach (var message in ReadMessages<RemoveCollisionShapeMessage>()) {
				if (message.Entity.Has<CollisionShapesInfo>()) {
					var shapes = message.Entity.Get<CollisionShapesInfo>().collisionShapes;

					shapes.Remove(message.CollisionShape);

					if (shapes.Count == 0) {
						message.Entity.Remove<CollisionShapesInfo>();
					}
				}
			}

			// Register collision shapes
			foreach (var message in ReadMessages<AddCollisionShapeMessage>()) {
				List<CollisionShape> shapes;

				if (!message.Entity.Has<CollisionShapesInfo>()) {
					shapes = new();

					message.Entity.Set(new CollisionShapesInfo {
						collisionShapes = shapes
					});
				} else {
					shapes = message.Entity.Get<CollisionShapesInfo>().collisionShapes;
				}

				shapes.Add(message.CollisionShape);
			}
		}
	}
}
