using System.Linq;

namespace Dissonance.Engine.Physics
{
	[AllowOnlyOnePerObject]
	public class RigidbodyBase : PhysicsComponent
	{
		protected bool isKinematic;
		protected bool useGravity;

		public bool WantsSleeping => gameObject.rigidbodyInternal.btRigidbody.WantsSleeping();

		public bool IsKinematic {
			get => isKinematic;
			set => gameObject.rigidbodyInternal.Type = (isKinematic = value) ? RigidbodyType.Kinematic : RigidbodyType.Dynamic;
		}
		public bool UseGravity {
			get => useGravity;
			set => gameObject.rigidbodyInternal.UseGravity = value;
		}
		public bool Active {
			get => gameObject.rigidbodyInternal.btRigidbody.IsActive;
			set {
				if(value) {
					gameObject.rigidbodyInternal?.btRigidbody?.Activate();
				}
			}
		}
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

		protected override void OnPreInit()
		{
			base.OnPreInit();

			gameObject.rigidbodyInternal.rigidbody = this;
			gameObject.rigidbodyInternal.Type = RigidbodyType.Dynamic;
		}
		protected override void OnEnable()
		{
			PhysicsEngine.ActiveRigidbodies.Add(this);

			IsKinematic = isKinematic;
		}
		protected override void OnDisable()
		{
			PhysicsEngine.ActiveRigidbodies.Remove(this);

			gameObject.rigidbodyInternal.Type = RigidbodyType.Static;
		}
		protected override void OnDispose()
		{
			base.OnDispose();

			OnDisable();
		}
	}
}
