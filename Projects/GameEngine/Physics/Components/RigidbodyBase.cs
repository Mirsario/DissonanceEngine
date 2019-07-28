namespace GameEngine
{
	public enum RigidbodyType
	{
		Dynamic,
		Kinematic,
		Static
	}
	[AllowOnlyOnePerObject]
	public class RigidbodyBase : PhysicsComponent
	{
		#region Properties
		public bool WantsSleeping => gameObject.rigidbodyInternal.btRigidbody.WantsSleeping;

		public float Mass {
			get => gameObject.rigidbodyInternal.Mass;
			set => gameObject.rigidbodyInternal.Mass = value;
		}
		public float Friction {
			get => gameObject.rigidbodyInternal.Friction;
			set => gameObject.rigidbodyInternal.Friction = value;
		}
		public float Drag {
			get => gameObject.rigidbodyInternal.Drag;
			set => gameObject.rigidbodyInternal.Drag = value;
		}
		public float AngularDrag {
			get => gameObject.rigidbodyInternal.AngularDrag;
			set => gameObject.rigidbodyInternal.AngularDrag = value;
		}
		public bool UseGravity {
			get => gameObject.rigidbodyInternal.UseGravity;
			set => gameObject.rigidbodyInternal.UseGravity = value;
		}
		protected bool _isKinematic;
		public bool IsKinematic {
			get => _isKinematic;
			set => gameObject.rigidbodyInternal.Type = (_isKinematic = value) ? RigidbodyType.Kinematic : RigidbodyType.Dynamic;
		}
		public bool Active {
			get => gameObject.rigidbodyInternal.btRigidbody.IsActive;
			set { 
				if(value) {
					gameObject.rigidbodyInternal.btRigidbody.Activate();
				}
			}
		}
		#endregion

		protected override void OnInit()
		{
			base.OnInit();

			gameObject.rigidbodyInternal.rigidbody = this;
			gameObject.rigidbodyInternal.Type = RigidbodyType.Dynamic;
		}
		protected override void OnEnable()
		{
			Physics.ActiveRigidbodies.Add(this);

			IsKinematic = _isKinematic;
		}
		protected override void OnDisable()
		{
			Physics.ActiveRigidbodies.Remove(this);

			gameObject.rigidbodyInternal.Type = RigidbodyType.Static;
		}
		protected override void OnDispose()
		{
			OnDisable();
		}
	}
}

