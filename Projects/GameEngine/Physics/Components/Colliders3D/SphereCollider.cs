using BulletSharp;

namespace GameEngine
{
	public class SphereCollider : Collider
	{
		public float radius = 1f;

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
			collShape = new SphereShape(radius);
			base.UpdateCollider();
		}
	}
}