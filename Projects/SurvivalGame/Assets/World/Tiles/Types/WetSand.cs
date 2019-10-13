using GameEngine;

namespace SurvivalGame
{
	public class WetSand : TileType
	{
		protected override string[] Variants => new[] {
			"WetSand1.png",
			"WetSand2.png",
			"WetSand3.png",
			"WetSand4.png"
		};

		public override PhysicMaterial GetMaterial(Vector3? atPoint = null) => PhysicMaterial.GetMaterial<SandPhysicMaterial>();
	}
}
