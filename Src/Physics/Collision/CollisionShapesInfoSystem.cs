using System.Collections.Generic;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Callback<PhysicsUpdateGroup>]
	[ExecuteAfter<ColliderUpdateGroup>]
	public sealed class CollisionShapesInfoSystem : GameSystem
	{
		protected override void Execute()
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
