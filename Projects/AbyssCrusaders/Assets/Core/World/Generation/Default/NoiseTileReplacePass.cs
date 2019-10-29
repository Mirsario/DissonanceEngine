using GameEngine;
using GameEngine.Utils;
using System;

namespace AbyssCrusaders.Core.Generation.Default
{
	public class NoiseTileReplacePass : GenPass
	{
		public ushort tileType;
		public Func<Tile,bool> canPlace;
		public Func<int,int,float> minValueGetter;
		public Func<int,int,FastNoise> noiseGetter;

		public NoiseTileReplacePass(ushort tileType,Func<Tile,bool> canPlace,Func<int,int,float> minValueGetter,Func<int,int,FastNoise> noiseGetter)
		{
			this.tileType = tileType;
			this.canPlace = canPlace;
			this.minValueGetter = minValueGetter;
			this.noiseGetter = noiseGetter;
		}

		public override void Run(int seed,int index,World world)
		{
			var noise = noiseGetter(seed,index);

			for(int x = 0;x<world.width;x++) {
				for(int y = 0;y<world.height;y++) {
					ref var tile = ref world[x,y];

					if(!canPlace(tile)) {
						continue;
					}

					float minValue = minValueGetter(x,y);
					if(noise.GetNoise(x,y)>=minValue) {
						tile.type = tileType;
					}
				}
			}
		}
	}
}
