using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class CubeObj2 : CubeObj
	{
		public override void OnInit()
		{
			base.OnInit();

			renderer.Mesh = PrimitiveMeshes.GenerateCube();
			renderer.Mesh.RecalculateNormals();
			renderer.Mesh.Apply();
		}
		public override void FixedUpdate()
		{
			var pos = Transform.Position;
			float waterLevel = world.GetWaterLevelAt(pos);
			pos.y = waterLevel;
			Transform.Position = pos;
		}
	}
}