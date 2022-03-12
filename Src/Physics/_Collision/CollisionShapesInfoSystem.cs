﻿using System.Collections.Generic;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Callback<PhysicsUpdateGroup>]
	[ExecuteAfter<ColliderUpdateGroup>]
	public sealed partial class CollisionShapesInfoSystem : GameSystem
	{
		[MessageSubsystem]
		partial void RemoveCollisionShapes(in RemoveCollisionShapeMessage message, [FromEntity] ref CollisionShapesInfo collisionShapesInfo)
		{
			var shapes = collisionShapesInfo.collisionShapes;

			shapes.Remove(message.CollisionShape);

			if (shapes.Count == 0) {
				message.Entity.Remove<CollisionShapesInfo>();
			}
		}

		[MessageSubsystem]
		partial void RegisterCollisionShapes(in AddCollisionShapeMessage message)
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
