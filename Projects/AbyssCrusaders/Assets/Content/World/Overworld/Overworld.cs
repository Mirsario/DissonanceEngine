using System.Collections.Generic;
using AbyssCrusaders.Content.Tiles.Natural;
using AbyssCrusaders.Content.Tiles.Natural.Ores;
using AbyssCrusaders.Content.Worlds.Common.GenPasses;
using AbyssCrusaders.Content.Worlds.Overworld.GenPasses;
using AbyssCrusaders.Core;
using AbyssCrusaders.Core.Generation;
using AbyssCrusaders.Core.Generation.Default;
using GameEngine.Utils;

namespace AbyssCrusaders.Content.Worlds.Overworld
{
	public class Overworld : World
	{
		public override void ModifyGenTasks(List<GenPass> list)
		{
			ushort dirt = TilePreset.GetTypeId<Dirt>();
			ushort grass = TilePreset.GetTypeId<Grass>();
			ushort stone = TilePreset.GetTypeId<Stone>();
			ushort clay = TilePreset.GetTypeId<Clay>();
			ushort sand = TilePreset.GetTypeId<Sand>();
			ushort copperOre = TilePreset.GetTypeId<CopperOre>();
			ushort ironOre = TilePreset.GetTypeId<IronOre>();
			ushort silverOre = TilePreset.GetTypeId<SilverOre>();
			ushort goldOre = TilePreset.GetTypeId<GoldOre>();

			int caveHeight = height/2+128;
			
			list.Add(new OverworldTerrainPass());
			//list.Add(new RandomTileChunksPass(null,stone,width*height/1024,8,24,(int)(height*0.75f),height)); //Random stone walls

			//list.Add(new RandomTileChunksPass(clay,clay,width*height/4096,64,256,0,(int)(height*0.65f)));
			//list.Add(new RandomTileChunksPass(sand,sand,width*height/4096,128,256,0,(int)(height*0.65f)));

			//list.Add(new TunnelsPass(width/20,10,35,3,5));

			list.Add(new CavePass());

			//Stone
			list.Add(new NoiseTileReplacePass(stone,tile => tile.type>0 && tile.type!=grass,(x,y) => 0.1f,(seed,index) => new FastNoise(seed^index) {
				NoiseType = FastNoise.NoiseTypes.CubicFractal,
				Frequency = 0.1f
			}));

			static FastNoise OreNoise(float frequency,int seed,int index)
			{
				var noise = new FastNoise(seed^index) {
					NoiseType = FastNoise.NoiseTypes.Cellular,
					Frequency = frequency
				};

				noise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Natural);
				noise.SetCellularJitter(0.3f);

				var subNoise = new FastNoise(seed^index+1) {
					NoiseType = FastNoise.NoiseTypes.WhiteNoise
				};

				noise.SetCellularReturnType(FastNoise.CellularReturnType.NoiseLookup);
				noise.SetCellularNoiseLookup(subNoise);

				return noise;
			};

			//Copper Ore
			list.Add(new NoiseTileReplacePass(copperOre,tile => tile.type>0,(x,y) => 0.970f,(seed,index) => OreNoise(0.2f,seed,index)));
			//Iron Ore
			list.Add(new NoiseTileReplacePass(ironOre,tile => tile.type>0,(x,y) => 0.980f,(seed,index) => OreNoise(0.2f,seed,index)));
			//Silver Ore
			list.Add(new NoiseTileReplacePass(silverOre,tile => tile.type>0,(x,y) => 0.985f,(seed,index) => OreNoise(0.2f,seed,index)));
			//Gold Ore
			list.Add(new NoiseTileReplacePass(goldOre,tile => tile.type>0,(x,y) => 0.990f,(seed,index) => OreNoise(0.2f,seed,index)));

			list.Add(new GrassPass());

			list.Add(new TileFramePass());
		}
	}
}
