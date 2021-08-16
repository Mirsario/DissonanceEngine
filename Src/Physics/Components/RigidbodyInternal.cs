using System.Collections.Generic;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	internal struct RigidbodyInternal //, IDisposable
	{
		public bool updateShapes;
		public bool ownsCollisionShape;
		public RigidbodyType rigidbodyType;

		public RigidBody BulletRigidbody { get; set; }
		public RigidbodyMotionState MotionState { get; set; }
		public CollisionShape CollisionShape { get; set; }
		public List<CollisionShape> CollisionShapes { get; set; }

		internal static RigidbodyInternal Create()
		{
			return new RigidbodyInternal {
				CollisionShapes = new(),
				rigidbodyType = RigidbodyType.Dynamic
			};
		}
	}
}
