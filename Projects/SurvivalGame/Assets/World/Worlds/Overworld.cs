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

			ushort sand = TileType.GetId<Sand>();
			ushort wetSand = TileType.GetId<WetSand>();
			ushort dirt = TileType.GetId<Dirt>();
			ushort grass = TileType.GetId<Grass>();
			ushort grassFlowers = TileType.GetId<GrassFlowers>();

			//Heights
			list.Add(new NoiseHeightmapGenPass(128f,heightmapFrequency));

			//Tile types
			list.Add(new CustomNoiseGenPass(heightmapFrequency,(ref Tile tile,float noiseValue) => {
				if(tile.height<=waterLevel+0.5f) {
					tile.type = wetSand;
				} else if(tile.height<=waterLevel+1.5f) {
					tile.type = sand;
				} else if(tile.height<=waterLevel+3.75f) {
					tile.type = dirt;
				} else {
					tile.type = Rand.Next(3)==0 ? grassFlowers : grass;
				}
			}));

			//Cliffs
			list.Add(new AngleTileTypeGenPass(waterLevel));

			//Deserts
			list.Add(new CustomNoiseGenPass(heightmapFrequency,(ref Tile tile,float noiseValue) => {
				if(tile.height>=waterLevel+5f) {
					if(noiseValue<0.4f) {
						tile.type = sand;
					} else if(noiseValue<0.42f) {
						tile.type = dirt;
					}
				}
			}));

			//Spruces
			list.Add(new NoiseChunkEntityGenPass<Spruce>(100,20,0.25f,heightmapFrequency,t => t.height>waterLevel+3f && t.type==grass));

			//Berry Bushes
			list.Add(new NoiseChunkEntityGenPass<BerryBush>(200,50,0.25f,heightmapFrequency,t => t.height>waterLevel+3f && (t.type==grass || t.type==sand)));

			//Boulders
			list.Add(new NoiseChunkEntityGenPass<Boulder>(400,50,0.5f,heightmapFrequency,t => t.height>waterLevel-2f));
		}
	}
}
