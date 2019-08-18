using GameEngine;
using GameEngine.Graphics;
using GameEngine.Extensions.Chains;

namespace AbyssCrusaders
{
	public class Skybox : GameObject2D
	{
		public override void OnInit()
		{
			AddComponent<Sprite>(c => c.Material = Resources.Get<Material>("Skybox.material"));

			Depth = -1000f;
		}
		public override void RenderUpdate()
		{
			if(Main.camera!=null) {
				Position = Main.camera.Position;
				Transform.LocalScale = new Vector3(Main.camera.rect.width,Main.camera.rect.height,1f);
			}
		}
	}
}