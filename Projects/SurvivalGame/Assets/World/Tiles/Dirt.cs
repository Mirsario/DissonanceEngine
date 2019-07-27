using GameEngine;

namespace SurvivalGame
{
	public class Dirt : TileType
	{
		public override PhysicMaterial GetMaterial(Vector3? atPoint = null) => PhysicMaterial.GetMaterial<DirtPhysicMaterial>();

		protected override string[] Variants => new[] { "Dirt1.png","Dirt2.png" };
	}
}
