using BulletSharp;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Physics
{
	public abstract class Collider : PhysicsComponent
	{
		internal CollisionShape collShape;

		protected Vector3 offset = Vector3.Zero;

		private bool needsUpdate;

		protected abstract bool OwnsShape { get; }

		public Vector3 Offset {
			get => offset;
			set {
				if(offset!=value) {
					offset = value;

					TryUpdateCollider();
				}
			}
		}

		internal virtual void UpdateCollider()
		{
			if(collShape!=null) {
				collShape.UserObject = this;
			}

			gameObject.rigidbodyInternal.UpdateShape();

			needsUpdate = false;
		}

		protected override void OnInit()
		{
			base.OnInit();

			needsUpdate = true;
		}
		protected override void OnEnable()
		{
			base.OnEnable();

			if(needsUpdate) {
				UpdateCollider();
			}
		}
		protected override void OnDispose()
		{
			base.OnDispose();

			/*if(OwnsShape) {
				collShape?.Dispose();
			}*/
		}

		protected void TryUpdateCollider()
		{
			if(enabled) {
				UpdateCollider();
			} else {
				needsUpdate = true;
			}
		}
	}
}
