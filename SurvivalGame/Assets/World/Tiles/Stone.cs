using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;

namespace SurvivalGame
{
	public class Stone : TileType
	{
		protected override string[] Variants => new[] { "Stone.png" };
		public override PhysicMaterial GetMaterial(Vector3? atPoint = null) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}
