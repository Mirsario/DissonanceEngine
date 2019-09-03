using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class HeightTileTypeGenPass : GenPass
	{
		public override void Run(World world,int seed)
		{
			ushort sand = TileType.byName["Sand"].type;
			ushort dirt = TileType.byName["Dirt"].type;
			ushort grass = TileType.byName["Grass"].type;
			ushort grassFlowers = TileType.byName["GrassFlowers"].type;

			for(int y = 0;y<world.ySize;y++) {
				for(int x = 0;x<world.xSize;x++) {
					ref var tile = ref world[x,y];

					if(tile.height<=world.waterLevel+1.5f) {
						tile.type = sand;
					} else if(tile.height<=world.waterLevel+3.75f) {
						tile.type = dirt;
					} else {
						tile.type = Rand.Next(3)==0 ? grassFlowers : grass;
					}
				}
			}
		}
	}
}
