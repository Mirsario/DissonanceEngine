/*using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public class Box2DCollider : Collider
	{
		protected Vector2 size = Vector2.One;

		public Vector2 Size {
			get => size;
			set {
				if(size!=value) {
					size = value;

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

			collShape = new Box2DShape(size.x*0.5f,size.y*0.5f,0.5f);

			base.UpdateCollider();
		}
	}
}*/
