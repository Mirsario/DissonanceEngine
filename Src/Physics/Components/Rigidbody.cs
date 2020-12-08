namespace Dissonance.Engine.Physics
{
	public class Rigidbody : RigidbodyBase
	{
		public Collision[] Collisions => gameObject.rigidbodyInternal.collisions.ToArray();

		public Vector3 Velocity {
			get => gameObject.rigidbodyInternal.Velocity;
			set => gameObject.rigidbodyInternal.Velocity = value;
		}
		public Vector3 AngularVelocity {
			get => gameObject.rigidbodyInternal.AngularVelocity;
			set => gameObject.rigidbodyInternal.AngularVelocity = value;
		}
		public Vector3 AngularFactor {
			get => gameObject.rigidbodyInternal.AngularFactor;
			set => gameObject.rigidbodyInternal.AngularFactor = value;
		}

		public void ApplyForce(Vector3 force, Vector3 relativePos)
		{
			if(!gameObject.rigidbodyInternal.btRigidbody.IsActive) {
				gameObject.rigidbodyInternal.btRigidbody.Activate();
			}

			gameObject.rigidbodyInternal.btRigidbody.ApplyForce(force, relativePos);
		}
	}
}
