using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class Overworld : World
	{
		public override void ModifyGenTasks(List<GenPass> list)
		{
			float heightmapFrequency = xSize*ySize/(1024f*256f);
			list.Add(new NoiseHeightmapGenPass(128f,heightmapFrequency));
			list.Add(new HeightTileTypeGenPass());
			list.Add(new AngleTileTypeGenPass(waterLevel));

			list.Add(new NoiseChunkEntityGenPass<Spruce>(100,10,0.25f,heightmapFrequency,new Vector2(waterLevel+3f,float.MaxValue)));
			list.Add(new NoiseChunkEntityGenPass<BerryBush>(800,100,0.25f,heightmapFrequency,new Vector2(waterLevel+3f,float.MaxValue)));
			list.Add(new NoiseChunkEntityGenPass<Boulder>(400,50,0.5f,heightmapFrequency,new Vector2(waterLevel-2f,float.MaxValue)));
		}
	}
}
