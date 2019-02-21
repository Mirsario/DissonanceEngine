using BulletSharp;

namespace GameEngine
{
	public class BoxCollider : Collider
	{
		public Vector3 size = Vector3.one;

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
			collShape = new BoxShape(size*0.5f);
			base.UpdateCollider();
		}
	}
}