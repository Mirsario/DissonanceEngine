using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public interface ICollider
	{
		internal CollisionShape collShape;

		protected Vector3 offset = Vector3.Zero;

		private bool needsUpdate;

		protected abstract bool OwnsShape { get; }

		public Vector3 Offset {
			get => offset;
			set {
				if(offset != value) {
					offset = value;

					TryUpdateCollider();
				}
			}
		}

		internal virtual void UpdateCollider()
		{
			if(collShape != null) {
				collShape.UserObject = this;
			}

			gameObject.rigidbodyInternal.UpdateShape();

			needsUpdate = false;
		}
	}
}
