/*using BulletSharp;

namespace GameEngine.Physics
{
	public class CapsuleCollider : Collider
	{
		protected float radius = 0.5f;
		protected float height = 2f;

		public float Radius {
			get => radius;
			set {
				if(radius!=value) {
					radius = value;

					TryUpdateCollider();
				}
			}
		}
		public float Height {
			get => height;
			set {
				if(height!=value) {
					height = value;

					TryUpdateCollider();
				}
			}
		}

		internal override void UpdateCollider()
		{
			if(collShape!=null) {
				collShape.Dispose();
				collShape = null;
			}

			collShape = new CapsuleShape(radius,height-2f*radius);

			base.UpdateCollider();
		}
	}
}*/
