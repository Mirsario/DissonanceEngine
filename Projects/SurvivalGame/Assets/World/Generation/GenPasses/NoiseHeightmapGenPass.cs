using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class NoiseHeightmapGenPass : GenPass
	{
		public float maxHeight;
		public float frequency;

		public NoiseHeightmapGenPass(float maxHeight,float frequency)
		{
			this.maxHeight = maxHeight;
			this.frequency = frequency;
		}

		public override void Run(World world,int seed)
		{
			var noise = new PerlinNoiseDouble(seed,8,frequency,0.5f);

			float divX = 1f/world.xSize;
			float divY = 1f/world.ySize;

			for(int y = 0;y<world.ySize;y++) {
				for(int x = 0;x<world.xSize;x++) {
					world[x,y].height = noise.GetValue(x*divX,0f,y*divY)*maxHeight;
				}
			}
		}
	}
}
