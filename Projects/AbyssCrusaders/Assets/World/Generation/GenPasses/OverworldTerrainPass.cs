using GameEngine;
using AbyssCrusaders.Tiles;

namespace AbyssCrusaders.Generation.GenPasses
{
	public class OverworldTerrainPass : GenPass
	{
		public override void Run(World world)
		{
			ushort dirt = TilePreset.GetTypeId<Dirt>();
			ushort grass = TilePreset.GetTypeId<Grass>();
			ushort stone = TilePreset.GetTypeId<Stone>();
			ushort wood = TilePreset.GetTypeId<Wood>();

			int halfWidth = world.width/2;
			int halfHeight = world.height/2;
			float xDiv = 1f/world.width;
			float yDiv = 1f/world.height;

			var perlinNoise = new PerlinNoiseFloat(frequency:world.width/512f,quality:PerlinNoiseFloat.QualityMode.High);
			//var caveNoise = new PerlinNoiseFloat(frequency:(world.width*world.height)/1024f,quality:PerlinNoiseFloat.QualityMode.High);

			const float SurfaceVariance = 128f;

			for(int x = 0;x<world.width;x++) {
				int surfaceLayer = halfHeight+(int)((perlinNoise.GetValue(x*xDiv,0f,0f)-0.5f)*SurfaceVariance);
				//int stoneLayer = (int)(Mathf.Min(1f,perlinNoise.GetValue(x*xDiv,0.5f,0f)*2f)*world.height);

				if(x==halfWidth) {
					world.spawnPoint = new Vector2Int(x,surfaceLayer-2);
				}
				
				for(int y = 0;y<world.height;y++) {
					ref var tile = ref world[x,y];

					if(y>surfaceLayer) { //caveNoise.GetValue(x*xDiv,y*yDiv,0f)>0.5f
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
