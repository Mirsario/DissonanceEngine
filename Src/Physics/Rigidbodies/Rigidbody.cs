using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public struct Rigidbody
	{
		public static readonly Rigidbody Default = new() {
			Mass = 1f
		};

		internal bool updateShapes;
		internal bool updateFlags;
		internal bool ownsCollisionShape;
		internal bool? wasKinematic;
		internal float lastMass;
		internal RigidbodyMotionState motionState;
		internal RigidBody bulletRigidbody;

		public float Mass { get; set; }
		public Vector3 Velocity { get; set; }
		public Vector3 AngularVelocity { get; set; }
		public Vector3 AngularFactor { get; set; }
		public RigidbodyType Type { get; set; }

		public Rigidbody(RigidbodyType type = RigidbodyType.Dynamic) : this()
		{
			Type = type;
		}
	}
}
