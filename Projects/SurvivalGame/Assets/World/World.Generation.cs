//#define LOOP_WORLD

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameEngine;
using GameEngine.Physics;

namespace SurvivalGame
{
	public partial class World
	{
		public abstract void ModifyGenTasks(List<GenPass> list);

		public void Generate(int seed)
		{
			ushort grassFlowers = TileType.byName["GrassFlowers"].type;
			ushort dirt = TileType.byName["Dirt"].type;
			ushort stone = TileType.byName["Stone"].type;

			waterLevel = 32f;

			float beachLevel = waterLevel+1.5f;

			var list = new List<GenPass>();

			ModifyGenTasks(list);

			for(int i = 0;i<list.Count;i++) {
				var task = list[i];

				Debug.Log($"Executing generation task {task.GetType().Name}...");

				task.Run(this,seed);

				Debug.Log("Done...");
			}

			/*for(int i=0;i<5;i++) {
				var posA = new Vector2Int(Rand.Next(xSize),Rand.Next(ySize));
				var posB = new Vector2Int(Rand.Next(xSize),Rand.Next(ySize));
				LineLoop(posA,posB,pos => this[pos.x,pos.y].type = dirt);
				LineLoop(posA+new Vector2Int(1,0),posB+new Vector2Int(1,0),pos => this[pos.x,pos.y].type = dirt);
			}*/

			var genRoster = new (int maxRand, Action<Tile,int,int,Vector3> action)[] {
				(25,(t,x,y,spawnPos) => {
					if(spawnPos.y<=beachLevel) {
						return;
					}
					Entity.Instantiate<Spruce>(this,position:spawnPos,rotation:Quaternion.FromEuler(0f,Rand.Range(0f,360f),0f));
					t.type = dirt;
				}),
				(60,(t,x,y,spawnPos) => {
					Entity.Instantiate<Boulder>(this,position:spawnPos,rotation:Quaternion.FromEuler(0f,Rand.Range(0f,360f),0f));
					for(int i=0;i<20;i++) {
						this[x+Rand.Range(-2,2),y+Rand.Range(-2,2)].type = dirt;
					}
				}),
				(300,(t,x,y,spawnPos) => {
					if(spawnPos.y<=beachLevel) {
						return;
					}
					Entity.Instantiate<BerryBush>(this,position:spawnPos);
					for(int i=0;i<20;i++) {
						this[x+Rand.Range(-2,2),y+Rand.Range(-2,2)].type = grassFlowers;
					}
					t.type = dirt;
				}),
				(600,(t,x,y,spawnPos) => {
					if(spawnPos.y<=beachLevel) {
						return;
					}

					Entity.Instantiate<Campfire>(this,position:spawnPos);
					t.type = dirt;
					for(int i = 0;i<3;i++) {
						Entity.Instantiate<StoneHatchet>(this,position:spawnPos+new Vector3(Rand.Range(-2f,2f),Rand.Range(5f,7f),Rand.Range(-2f,2f)));
					}
				}),
			};


			//Random stone pikes
			int spikeAmount = xSize*ySize/200;
			for(int i = 0;i<spikeAmount;i++) {
				int x = Rand.Range(1,xSize-1);
				int y = Rand.Range(1,ySize-1);
				ref var tile = ref tiles[x,y];
				if(tile.type==dirt || tile.type==stone) {
					tile.height += Rand.Range(-1f,4.75f);
					tiles[x-1,y].type = tiles[x,y-1].type = tiles[x-1,y-1].type = tile.type = stone;
				}
			}

			var playerPos = new Vector3(xSizeInUnits*0.5f,0f,ySizeInUnits*0.5f);
			playerPos.y = HeightAt(playerPos,false);

			Main.LocalEntity = Entity.Instantiate<Human>(this,position: playerPos); //Instantiate<Human>(null,new Vector3(xSizeInUnits*0.5f,56f,ySizeInUnits*0.5f));

			Entity.Instantiate<StoneHatchet>(this,position: new Vector3(xSizeInUnits*0.5f-1f,45f,ySizeInUnits*0.5f));
			Instantiate<AtmosphereSystem>();
			Instantiate<Sun>();
			Instantiate<Skybox>(); //TODO: Implement a skybox inside the engine
			Entity.Instantiate<Water>(this,position: new Vector3(xSizeInUnits*0.5f,32f,ySizeInUnits*0.5f));

			IsReady = true;
		}
	}
}

