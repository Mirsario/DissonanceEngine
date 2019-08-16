using GameEngine;

namespace SurvivalGame
{
	public class Sand : TileType
	{
		protected override string[] Variants => new[] {
			"Sand1.png",
			"Sand2.png",
			"Sand3.png",
			"Sand4.png"
		};

		public override PhysicMaterial GetMaterial(Vector3? atPoint = null) => PhysicMaterial.GetMaterial<DirtPhysicMaterial>();
	}
}
