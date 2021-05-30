using System;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public readonly struct AddCollisionShapeMessage : IMessage
	{
		public readonly Entity Entity;
		public readonly CollisionShape CollisionShape;

		public AddCollisionShapeMessage(Entity entity, CollisionShape collisionShape)
		{
			Entity = entity;
			CollisionShape = collisionShape ?? throw new ArgumentNullException(nameof(collisionShape));
		}
	}
}
