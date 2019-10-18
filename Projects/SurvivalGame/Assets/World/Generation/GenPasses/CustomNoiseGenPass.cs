using GameEngine;
using GameEngine.Utils;
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
			var noise = new FastNoise(seed^index) {
				Frequency = frequency
			};

			for(int y = 0;y<world.ySize;y++) {
				for(int x = 0;x<world.xSize;x++) {
					tileAction(ref world[x,y],noise.GetNoise(x,y)+0.5f);
				}
			}
		}
	}
}
