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
		internal bool updateMass;
		internal bool ownsCollisionShape;
		internal RigidbodyMotionState motionState;
		internal RigidBody bulletRigidbody;

		private float mass;

		public RigidbodyType Type { get; set; }

		public float Mass {
			get => mass;
			set {
				mass = value;
				updateMass = true;
			}
		}
		public Vector3 Velocity {
			get => bulletRigidbody.LinearVelocity;
			set => bulletRigidbody.LinearVelocity = value;
		}
		public Vector3 AngularVelocity {
			get => bulletRigidbody.AngularVelocity;
			set => bulletRigidbody.AngularVelocity = value;
		}
		public Vector3 AngularFactor {
			get => bulletRigidbody.AngularFactor;
			set => bulletRigidbody.AngularFactor = value;
		}

		public Rigidbody(RigidbodyType type = RigidbodyType.Dynamic) : this()
		{
			Type = type;
		}
	}
}
