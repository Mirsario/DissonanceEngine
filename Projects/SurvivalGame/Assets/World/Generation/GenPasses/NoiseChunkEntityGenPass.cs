using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class NoiseChunkEntityGenPass<T> : GenPass where T : Entity
	{
		public int minChance;
		public int maxChance;
		public float minValue;
		public float frequency;
		public Vector2 heightRange;
		public Func<Tile,bool> canPlaceFunc;

		public NoiseChunkEntityGenPass(int minChance,int maxChance,float minValue,float frequency,Func<Tile,bool> canPlaceFunc = null)
		{
			this.minChance = minChance;
			this.maxChance = maxChance;
			this.minValue = minValue;
			this.frequency = frequency;
			this.canPlaceFunc = canPlaceFunc;
		}

		public override void Run(World world,int seed,int index)
		{
			var noise = new PerlinNoiseFloat(seed^index,8,frequency,0.5f);

			float divX = 1f/world.xSize;
			float divY = 1f/world.ySize;

			for(int y = 0;y<world.ySize;y++) {
				for(int x = 0;x<world.xSize;x++) {
					var tile = world[x,y];

					if(canPlaceFunc!=null && !canPlaceFunc(tile)) {
						continue;
					}

					float noiseValue = noise.GetValue(x*divX,0f,y*divY);
					if(noiseValue>=minValue) {
						int chance = Mathf.RoundToInt(Mathf.Lerp(minChance,maxChance,(noiseValue-minValue)*(1f/(1f-minValue))));

						if(Rand.Next(chance)==0) {
							var spawnPos = new Vector3(x*Chunk.TileSize+Chunk.TileSizeHalf,0f,y*Chunk.TileSize+Chunk.TileSizeHalf);
							spawnPos.y = world.HeightAt(spawnPos,false);

							Entity.Instantiate<T>(world,position: spawnPos);
						}
					}
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
