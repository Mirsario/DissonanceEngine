using System.Collections.Generic;
using AbyssCrusaders.Generation.GenPasses;

namespace AbyssCrusaders
{
	public class Overworld : World
	{
		public override void ModifyGenTasks(List<GenPass> list)
		{
			ushort dirt = TilePreset.GetTypeId<Tiles.Dirt>();
			ushort stone = TilePreset.GetTypeId<Tiles.Stone>();
			ushort clay = TilePreset.GetTypeId<Tiles.Clay>();
			ushort sand = TilePreset.GetTypeId<Tiles.Sand>();
			ushort copperOre = TilePreset.GetTypeId<Tiles.CopperOre>();
			ushort ironOre = TilePreset.GetTypeId<Tiles.IronOre>();
			ushort silverOre = TilePreset.GetTypeId<Tiles.SilverOre>();
			ushort goldOre = TilePreset.GetTypeId<Tiles.GoldOre>();
			
			list.Add(new OverworldTerrainPass());
			list.Add(new GrassPass());
			list.Add(new RandomTileChunksPass(null,stone,width*height/1024,8,24,(int)(height*0.75f),height)); //Random stone walls
			//list.Add(new RandomTileChunksPass(clay,clay,width*height/4096,64,256,0,(int)(height*0.65f)));
			//list.Add(new RandomTileChunksPass(sand,sand,width*height/4096,128,256,0,(int)(height*0.65f)));
			list.Add(new RandomTileChunksPass(stone,stone,width*height/32,3,10,0,height)); //Random stone tiles & walls
			//list.Add(new RandomTileChunksPass(copperOre,stone,width*height/512,4,24,0,height));
			//list.Add(new RandomTileChunksPass(ironOre,stone,width*height/768,4,24,0,height));
			//list.Add(new RandomTileChunksPass(silverOre,stone,width*height/1024,8,16,(int)(height*0.65f),height));
			//list.Add(new RandomTileChunksPass(goldOre,stone,width*height/2048,8,24,(int)(height*0.75f),height));
			list.Add(new TunnelsPass(width/20,10,35,3,5));
			list.Add(new TileFramePass());
		}
	}
}
