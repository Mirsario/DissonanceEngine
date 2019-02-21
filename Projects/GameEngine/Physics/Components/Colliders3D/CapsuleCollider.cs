using BulletSharp;

namespace GameEngine
{
	public class CapsuleCollider : Collider
	{
		public float radius = 0.5f;
		public float height = 2f;

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
			collShape = new CapsuleShape(radius,height-2f*radius);
			base.UpdateCollider();
		}
	}
}