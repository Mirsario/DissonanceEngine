using System.Collections.Generic;
using BulletSharp;

namespace Dissonance.Engine.Physics;

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
		ref var collisionShapesInfo = ref message.Entity.GetOrSet(() => new CollisionShapesInfo());
		var shapes = collisionShapesInfo.collisionShapes ??= new();

		shapes.Add(message.CollisionShape);
	}
}
