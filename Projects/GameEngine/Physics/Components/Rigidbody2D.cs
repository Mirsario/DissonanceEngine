namespace GameEngine
{
	public class Rigidbody2D : RigidbodyBase
	{
		#region Properties
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
		/*public float AngularFactor {
			get => GameObject.rigidbodyInternal.AngularFactor.x;
			set => GameObject.rigidbodyInternal.AngularFactor = new Vector3(value,0f,0f);
		}*/
		#endregion

		protected override void OnEnable()
		{
			base.OnEnable();
			gameObject.rigidbodyInternal.btRigidbody.LinearFactor = new Vector3(1f,1f,0f);
		}
		public void ApplyForce(Vector2 force,Vector2 relativePos)
		{
			if(!gameObject.rigidbodyInternal.btRigidbody.IsActive) {
				gameObject.rigidbodyInternal.btRigidbody.Activate();
			}
			gameObject.rigidbodyInternal.btRigidbody.ApplyForce(new Vector3(force.x,force.y,0f),new Vector3(relativePos.x,relativePos.y,0f));
		}
	}
}

