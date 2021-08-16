using System.Collections.Generic;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	internal struct RigidbodyInternal
	{
		public bool UpdateShapes;
		public bool UpdateFlags;
		public bool OwnsCollisionShape;
		public RigidbodyType Type;

		public RigidBody BulletRigidbody { get; set; }
		public RigidbodyMotionState MotionState { get; set; }
		public CollisionShape CollisionShape { get; set; }
		public List<CollisionShape> CollisionShapes { get; set; }

		internal static RigidbodyInternal Create()
		{
			return new RigidbodyInternal {
				CollisionShapes = new()
			};
		}
	}
}
