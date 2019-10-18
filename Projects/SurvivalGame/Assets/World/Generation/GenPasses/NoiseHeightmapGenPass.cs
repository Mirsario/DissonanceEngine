using GameEngine;
using GameEngine.Graphics;
using GameEngine.Utils;
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

		public override void Run(World world,int seed,int index)
		{
			//var noise = new PerlinNoise(seed^index,8,frequency,0.5f);
			var noise = new FastNoise(seed^index) {
				Frequency = 0.01f,
				NoiseType = FastNoise.NoiseTypes.CubicFractal,
				FractalOctaves = 5
			};

			//float divX = 1f/world.xSize;
			//float divY = 1f/world.ySize;

			//var pixels = new Pixel[world.xSize,world.ySize];

			for(int y = 0;y<world.ySize;y++) {
				for(int x = 0;x<world.xSize;x++) {
					world[x,y].height = (noise.GetNoise(x,y)+0.5f)*maxHeight;

					/*ref var pixel = ref pixels[x,y];

					pixel.r = pixel.g = pixel.b = (byte)Mathf.Clamp((int)(255f*noiseValue),0,255);
					pixel.a = 255;*/
				}
			}

			/*using var tex = new Texture(world.xSize,world.ySize);
			tex.SetPixels(pixels);
			tex.Save("heightmap.png");*/
		}
	}
}
