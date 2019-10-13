using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class CustomNoiseGenPass : GenPass
	{
		public delegate void TileAction(ref Tile existingTile,float noiseValue);

		public float frequency;
		public TileAction tileAction;

		public CustomNoiseGenPass(float frequency,TileAction tileAction)
		{
			this.frequency = frequency;
			this.tileAction = tileAction;
		}

		public override void Run(World world,int seed,int index)
		{
			var noise = new PerlinNoiseDouble(seed^index,8,frequency,0.5f);

			float divX = 1f/world.xSize;
			float divY = 1f/world.ySize;

			for(int y = 0;y<world.ySize;y++) {
				for(int x = 0;x<world.xSize;x++) {
					ref var tile = ref world[x,y];

					tileAction(ref tile,noise.GetValue(x*divX,0f,y*divY));
				}
			}
		}
	}
}
