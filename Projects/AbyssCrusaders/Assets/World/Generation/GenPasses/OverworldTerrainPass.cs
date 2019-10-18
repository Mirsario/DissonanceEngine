using GameEngine;
using AbyssCrusaders.Tiles;
using GameEngine.Utils;

namespace AbyssCrusaders.Generation.GenPasses
{
	public class OverworldTerrainPass : GenPass
	{
		public override void Run(int seed,int index,World world)
		{
			ushort dirt = TilePreset.GetTypeId<Dirt>();
			//ushort grass = TilePreset.GetTypeId<Grass>();
			//ushort stone = TilePreset.GetTypeId<Stone>();
			//ushort wood = TilePreset.GetTypeId<Wood>();

			int halfWidth = world.width/2;
			int halfHeight = world.height/2;

			var noise = new FastNoise(seed^index) {
				NoiseType = FastNoise.NoiseTypes.CubicFractal,
				FractalType = FastNoise.FractalTypes.Billow,
				Frequency = 0.003f,
				FractalOctaves = 5
			};

			//var caveNoise = new PerlinNoiseFloat(frequency:(world.width*world.height)/1024f,quality:PerlinNoiseFloat.QualityMode.High);

			const float SurfaceVariance = 128f;

			for(int x = 0;x<world.width;x++) {
				int surfaceLayer = halfHeight+(int)(noise.GetNoise(x,0f)*SurfaceVariance);
				//int stoneLayer = (int)(Mathf.Min(1f,perlinNoise.GetValue(x*xDiv,0.5f,0f)*2f)*world.height);

				if(x==halfWidth) {
					world.spawnPoint = new Vector2Int(x,surfaceLayer-2);
				}
				
				for(int y = 0;y<world.height;y++) {
					ref var tile = ref world[x,y];

					if(y>surfaceLayer) {
						tile.type = dirt;

						if(y>surfaceLayer+1) {
							tile.wall = dirt;
						}
					}
				}
			}
		}
	}
}
