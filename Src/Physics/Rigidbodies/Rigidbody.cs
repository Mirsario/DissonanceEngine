using System;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public struct Rigidbody
	{
		[Flags]
		internal enum UpdateFlags : byte
		{
			CollisionFlags = 1,
			CollisionShapes = 2,
			Mass = 4,
		}

		public static readonly Rigidbody Default = new() {
			Mass = 1f
		};

		internal bool ownsCollisionShape;
		internal UpdateFlags updateFlags;
		internal RigidbodyMotionState motionState;
		internal RigidBody bulletRigidbody;

		private float mass;
		private RigidbodyType type;

		public RigidbodyType Type {
			get => type;
			set {
				type = value;
				updateFlags |= UpdateFlags.CollisionFlags;
			}
		}
		public float Mass {
			get => mass;
			set {
				mass = value;
				updateFlags |= UpdateFlags.Mass;
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
