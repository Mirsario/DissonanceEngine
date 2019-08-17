using BulletSharp;

namespace GameEngine
{
	public abstract class Collider : PhysicsComponent
	{
		internal CollisionShape collShape;

		private bool needsUpdate;

		protected Vector3 offset = Vector3.Zero;
		public Vector3 Offset {
			get => offset;
			set {
				if(offset!=value) {
					offset = value;

					TryUpdateCollider();
				}
			}
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

		internal virtual void UpdateCollider()
		{
			if(collShape!=null) {
				collShape.UserObject = this;
			}

			gameObject.rigidbodyInternal.UpdateShape();
			needsUpdate = false;
		}

		protected void TryUpdateCollider()
		{
			if(enabled) {
				UpdateCollider();
			}else{
				needsUpdate = true;
			}
		}
	}
}

