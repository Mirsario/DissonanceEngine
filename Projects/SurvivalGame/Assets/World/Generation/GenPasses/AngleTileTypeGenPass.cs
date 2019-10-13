using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class AngleTileTypeGenPass : GenPass
	{
		public float minHeight;

		public AngleTileTypeGenPass(float minHeight)
		{
			this.minHeight = minHeight;
		}

		public override void Run(World world,int seed,int index)
		{
			ushort dirt = TileType.byName["Dirt"].type;
			ushort stone = TileType.byName["Stone"].type;

			for(int y = 0;y<world.ySize;y++) {
				for(int x = 0;x<world.xSize;x++) {
					ref Tile tile = ref world[x,y];

					if(tile.height>=minHeight) {
						float maxHeightDiff = Mathf.Max(Mathf.Abs(world[x-1,y].height-world[x+1,y].height),Mathf.Abs(world[x,y-1].height-world[x,y+1].height));
						if(maxHeightDiff>=3.2f) {
							tile.type = maxHeightDiff>=4.35f ? stone : dirt;
							continue;
						}
					}
				}
			}
		}
	}
}
