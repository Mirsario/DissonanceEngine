using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders
{
	public class Skybox : GameObject2D
	{
		public override void OnInit()
		{
			AddComponent<Sprite>(c => {
				var mat = Resources.Get<Material>("Skybox.material");
				//mat.SetVector3("colorTop",Vector3.Zero);
				//mat.SetVector3("colorBottom",new Vector3(59,36,93)/255f);
				c.Material = mat;
			});

			Depth = -1000f;
		}
		public override void RenderUpdate()
		{
			if(Main.camera!=null) {
				Position = Main.camera.Position;
				Transform.LocalScale = new Vector3(Main.camera.zoomedRectInPixels.width,Main.camera.zoomedRectInPixels.height,1f);
			}
		}
	}
}