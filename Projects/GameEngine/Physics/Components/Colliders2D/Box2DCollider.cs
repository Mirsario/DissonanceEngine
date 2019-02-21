using BulletSharp;

namespace GameEngine
{
	public class Box2DCollider : Collider
	{
		public Vector2 size = Vector2.One;

		protected override void OnInit()
		{
			base.OnInit();
			UpdateCollider();
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
}