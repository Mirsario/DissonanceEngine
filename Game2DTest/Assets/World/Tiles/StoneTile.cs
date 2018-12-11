using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game2DTest.Tiles
{
	public class Stone : TileType
	{
		protected override string[] Variants => new[] { "Stone1.png","Stone2.png","Stone3.png","Stone4.png" };
	}
}
