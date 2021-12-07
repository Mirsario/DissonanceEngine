using System.Collections.Generic;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Reads<CollisionShapesInfo>]
	[Writes<CollisionShapesInfo>]
	[Receives<AddCollisionShapeMessage>]
	[Receives<RemoveCollisionShapeMessage>]
	public sealed partial class CollisionShapesInfoSystem : GameSystem
	{
		[MessageSubsystem]
		private static partial void RemoveCollisionShapes(in RemoveCollisionShapeMessage message, [FromEntity] ref CollisionShapesInfo collisionShapesInfo)
		{
			var shapes = collisionShapesInfo.collisionShapes;

			shapes.Remove(message.CollisionShape);

			if (shapes.Count == 0) {
				message.Entity.Remove<CollisionShapesInfo>();
			}
		}

		[MessageSubsystem]
		private static partial void RegisterCollisionShapes(in AddCollisionShapeMessage message)
		{
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
