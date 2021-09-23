using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

		public static readonly Rigidbody Default = new();

		internal bool ownsCollisionShape;
		internal bool collisionsHaveBeenModified;
		internal UpdateFlags updateFlags;
		internal RigidBody bulletRigidbody;
		internal Vector3? pendingVelocity;
		internal Vector3? pendingAngularVelocity;
		internal Vector3? pendingAngularFactor;
		internal List<Collision> collisions;

		private float mass = 1f;
		private RigidbodyType type = RigidbodyType.Dynamic;

		public float Friction { get; set; } = 0.5f;

		public ReadOnlySpan<Collision> Collisions => CollectionsMarshal.AsSpan(collisions);

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
			get => bulletRigidbody?.LinearVelocity ?? pendingVelocity ?? default;
			set {
				if (bulletRigidbody != null) {
					bulletRigidbody.LinearVelocity = value;
				} else {
					pendingVelocity = value;
				}
			}
		}
		public Vector3 AngularVelocity {
			get => bulletRigidbody?.AngularVelocity ?? pendingAngularVelocity ?? default;
			set {
				if (bulletRigidbody != null) {
					bulletRigidbody.AngularVelocity = value;
				} else {
					pendingAngularVelocity = value;
				}
			}
		}
		public Vector3 AngularFactor {
			get => bulletRigidbody?.AngularFactor ?? pendingAngularFactor ?? default;
			set {
				if (bulletRigidbody != null) {
					bulletRigidbody.AngularFactor = value;
				} else {
					pendingAngularFactor = value;
				}
			}
		}

		/*public Rigidbody(RigidbodyType type) : this()
		{
			this.type = type;
		}*/
	}
}
