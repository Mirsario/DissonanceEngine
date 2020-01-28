/*using BulletSharp;

namespace GameEngine.Physics
{
	public class SphereCollider : Collider
	{
		protected float radius = 1f;
		public float Radius {
			get => radius;
			set {
				if(radius!=value) {
					radius = value;

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

			collShape = new SphereShape(radius);

			base.UpdateCollider();
		}
	}
}*/
