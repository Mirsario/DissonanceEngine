using GameEngine;
using GameEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class NoiseChunkEntityGenPass<T> : GenPass where T : Entity
	{
		public delegate void OnPlace(int x,int y,ref Tile tile,T obj);

		public int minChance;
		public int maxChance;
		public float minValue;
		public float frequency;
		public Vector2 heightRange;
		public Func<Tile,bool> canPlace;
		public OnPlace onPlace;

		public NoiseChunkEntityGenPass(int minChance,int maxChance,float minValue,float frequency,Func<Tile,bool> canPlace = null,OnPlace onPlace = null)
		{
			this.minChance = minChance;
			this.maxChance = maxChance;
			this.minValue = minValue;
			this.frequency = frequency;
			this.canPlace = canPlace;
			this.onPlace = onPlace;
		}

		public override void Run(World world,int seed,int index)
		{
			var noise = new FastNoise(seed^index) {
				Frequency = frequency
			};

			for(int y = 0;y<world.ySize;y++) {
				for(int x = 0;x<world.xSize;x++) {
					ref var tile = ref world[x,y];

					if(canPlace!=null && !canPlace(tile)) {
						continue;
					}

					float noiseValue = noise.GetNoise(x,y)+0.5f;
					if(noiseValue<minValue) {
						continue;
					}

					int chance = Mathf.RoundToInt(Mathf.Lerp(minChance,maxChance,(noiseValue-minValue)*(1f/(1f-minValue))));

					if(Rand.Next(chance)!=0) {
						continue;
					}

					var spawnPos = new Vector3(x*Chunk.TileSize+Chunk.TileSizeHalf,0f,y*Chunk.TileSize+Chunk.TileSizeHalf);
					spawnPos.y = world.HeightAt(spawnPos,false);

					var entity = Entity.Instantiate<T>(world,position: spawnPos);

					onPlace?.Invoke(x,y,ref tile,entity);
				}
			}

			/*for(int y = 0;y<world.ySize;y += step) {
				for(int x = 0;x<world.xSize;x += step) {
					float noiseValue = noise.GetValue(x*divX,0f,y*divY);
					if(noiseValue>=minValue) {
						var spawnPos = new Vector3(x*Chunk.TileSize+Chunk.TileSizeHalf,0f,y*Chunk.TileSize+Chunk.TileSizeHalf);
						spawnPos.y = world.HeightAt(spawnPos,false);

						Entity.Instantiate<T>(world,position:spawnPos);
					}
				}
			}*/
		}
	}
}
