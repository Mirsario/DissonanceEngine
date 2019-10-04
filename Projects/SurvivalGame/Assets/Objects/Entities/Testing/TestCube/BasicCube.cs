using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;
using ImmersionFramework;

namespace SurvivalGame
{
	public class BasicCube : GameObject
	{
		public MeshRenderer renderer;

		public override void OnInit()
		{
			renderer = AddComponent<MeshRenderer>(c => {
				c.Mesh = PrimitiveMeshes.Cube;
				c.Material = Resources.Get<Material>("TestCube.material");
			});

			Transform.LocalScale = Vector3.One*0.25f;
		}
	}
}