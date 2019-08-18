using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class Skybox : GameObject
	{
		public override void OnInit()
		{
			AddComponent<MeshRenderer>(c => {
				c.Mesh = PrimitiveMeshes.InvertedCube;
				c.Material = Resources.Get<Material>("Skybox.material");
			});
		}
		public override void RenderUpdate()
		{
			Transform.Position = Main.camera.Transform.Position;
			Transform.LocalScale = Vector3.One*Main.camera.farClip;
		}
	}
}