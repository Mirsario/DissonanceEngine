using GameEngine;

namespace SurvivalGame
{
	public class Stone : TileType
	{
		protected override string[] Variants => new[] { "Stone.png" };
		public override PhysicMaterial GetMaterial(Vector3? atPoint = null) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}
