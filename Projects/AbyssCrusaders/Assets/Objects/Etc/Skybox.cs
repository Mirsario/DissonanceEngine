using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders
{
	public class Skybox : GameObject2D
	{
		public Sprite sprite;
		
		public override void OnInit()
		{
			sprite = AddComponent<Sprite>();
			sprite.Material = Resources.Get<Material>("Skybox.material");

			Depth = -1000f;
		}
		public override void RenderUpdate()
		{
			if(Main.camera!=null) {
				Position = Main.camera.Position;
				Vector3 localScale = new Vector3(Main.camera.rect.width,Main.camera.rect.height,1f);
				Transform.LocalScale = localScale;
			}
		}
	}
}