using AbyssCrusaders.Core;
using AbyssCrusaders.Core.Generation;
using GameEngine;
using GameEngine.Utils;

namespace AbyssCrusaders.Content.Worlds.Common.GenPasses
{
	public class CavePass : GenPass
	{
		public ushort tileType;

		public CavePass(ushort tileType = 0)
		{
			this.tileType = tileType;
		}
		
		public override void Run(int seed,int index,World world)
		{
			/*var noiseA = new FastNoise {
				NoiseType = FastNoise.NoiseTypes.CubicFractal,
				Frequency = 0.05f
			};*/

			var noiseB = new FastNoise {
				NoiseType = FastNoise.NoiseTypes.CubicFractal,
				Frequency = 0.06f,
				FractalOctaves = 3,
				FractalType = FastNoise.FractalTypes.Billow,
				FractalGain = 0.25f
			};

			for(int x = 0;x<world.width;x++) {
				for(int y = 0;y<world.height;y++) {
					ref var tile = ref world[x,y];

					if(noiseB.GetNoise(x,y)+0.5f>0.02f) {
						tile.type = tileType;
					}
				}
			}
		}
	}
}
