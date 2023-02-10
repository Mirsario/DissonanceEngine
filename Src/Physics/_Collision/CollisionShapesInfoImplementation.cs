namespace Dissonance.Engine.Physics;

internal static partial class CollisionShapesInfoImplementation
{
	[MessageSystem, CalledIn<PhysicsUpdate>, Tags("CollisionShapesInfo"), RequiresTags("ColliderUpdate")]
	static partial void RemoveCollisionShapes(in RemoveCollisionShapeMessage message, [FromEntity] ref CollisionShapesInfo collisionShapesInfo)
	{
		var shapes = collisionShapesInfo.collisionShapes;

		shapes.Remove(message.CollisionShape);

		if (shapes.Count == 0) {
			message.Entity.Remove<CollisionShapesInfo>();
		}
	}

	[MessageSystem, CalledIn<PhysicsUpdate>, Tags("CollisionShapesInfo"), RequiresTags("ColliderUpdate")]
	static partial void RegisterCollisionShapes(in AddCollisionShapeMessage message)
	{
		ref var collisionShapesInfo = ref message.Entity.GetOrSet(() => new CollisionShapesInfo());
		var shapes = collisionShapesInfo.collisionShapes ??= new();

		shapes.Add(message.CollisionShape);
	}
}
