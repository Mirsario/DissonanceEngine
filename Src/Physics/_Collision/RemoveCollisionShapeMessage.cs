using System;
using BulletSharp;

namespace Dissonance.Engine.Physics;

public readonly struct RemoveCollisionShapeMessage
{
	public readonly Entity Entity;
	public readonly CollisionShape CollisionShape;

	public RemoveCollisionShapeMessage(Entity entity, CollisionShape collisionShape)
	{
		Entity = entity;
		CollisionShape = collisionShape ?? throw new ArgumentNullException(nameof(collisionShape));
	}
}
