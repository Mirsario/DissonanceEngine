/*namespace GameEngine.Physics
{
	public class Rigidbody2D : RigidbodyBase
	{
		public Collision2D[] Collisions => gameObject.rigidbodyInternal.collisions2D.ToArray();

		public Vector2 Velocity {
			get => gameObject.rigidbodyInternal.Velocity.XY;
			set {
				var rb = gameObject.rigidbodyInternal;
				var vel = rb.Velocity;
				vel.XY = value;
				rb.Velocity = vel;
			}
		}
		public float AngularVelocity {
			get => gameObject.rigidbodyInternal.AngularVelocity.x;
			set => gameObject.rigidbodyInternal.AngularVelocity = new Vector3(gameObject.rigidbodyInternal.AngularVelocity.x,0f,0f);
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			gameObject.rigidbodyInternal.btRigidbody.LinearFactor = new BulletSharp.Vector3(1f,1f,0f);
		}
		public void ApplyForce(Vector2 force,Vector2 relativePos)
		{
			var btRigidbody = gameObject.rigidbodyInternal.btRigidbody;

			if(!btRigidbody.IsActive) {
				btRigidbody.Activate();
			}

			btRigidbody.ApplyForce(new BulletSharp.Vector3(force.x,force.y,0f),new BulletSharp.Vector3(relativePos.x,relativePos.y,0f));
		}
	}
}*/
