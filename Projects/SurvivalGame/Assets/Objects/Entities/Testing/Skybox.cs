using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class Skybox : GameObject
	{
		private static Skybox instance;

		public override void OnInit()
		{
			instance = this;

			AddComponent<MeshRenderer>(c => {
				c.Mesh = PrimitiveMeshes.InvertedCube;
				c.Material = Resources.Get<Material>("Skybox.material");
			});
		}

		public static void OnRenderStart(Camera camera)
		{
			if(instance!=null) {
				var t = instance.Transform;
				t.Position = camera.Transform.Position;
				t.LocalScale = Vector3.One*camera.farClip;
			}
		}
	}
}