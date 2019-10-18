using System;
using System.Collections.Generic;
using GameEngine;

namespace AbyssCrusaders.Generation.GenPasses
{
	public class RandomTileChunksPass : GenPass
	{
		public ushort tileType;
		public ushort wallType;
		public bool placeTiles;
		public bool placeWalls;
		public int numChunks;
		public int minSize;
		public int maxSize;
		public int minY;
		public int maxY;

		public RandomTileChunksPass(ushort? tileType,ushort? wallType,int numChunks,int minSize,int maxSize,int minY,int maxY)
		{
			if(tileType.HasValue) {
				placeTiles = true;
				this.tileType = tileType.Value;
			}
			if(wallType.HasValue) {
				placeWalls = true;
				this.wallType = wallType.Value;
			}
			this.numChunks = numChunks;
			this.minSize = minSize;
			this.maxSize = maxSize;
			this.minY = minY;
			this.maxY = maxY;
		}
		
		public override void Run(int seed,int index,World world)
		{
			for(int i = 0;i<numChunks;i++) {
				int x = Rand.Next(world.width);
				int y = Rand.Range(minY,maxY);
				int size = Rand.Range(minSize,maxSize+1);

				var positions = new List<Vector2Int> {
					new Vector2Int(x,y)
				};
				for(int step = 0;step<positions.Count;step++) {
					var pos = positions[step];
					if(pos.x<0 || pos.y<0 || pos.x>=world.width || pos.y>=world.height) {
						continue;
					}
					ref var tile = ref world[pos.x,pos.y];
					if(tile.type==0) {
						break;
					}
					if(placeTiles) {
						tile.type = tileType;
					}
					if(placeWalls) {
						tile.wall = wallType;
					}
					
					int maxSpreads = Math.Min(4,size-step);
					if(maxSpreads<=0) {
						break;
					}
					int numSpreads = Rand.Range(1,maxSpreads);
					for(int k=0;k<numSpreads;k++) {
						void TryAdd(int X,int Y) {
							Vector2Int vec = new Vector2Int(X,Y);
							if(!positions.Contains(vec)) {
								positions.Add(vec);
							}
						}
						switch(Rand.Next(4)) {
							case 0: TryAdd(pos.x-1,pos.y); continue;
							case 1: TryAdd(pos.x+1,pos.y); continue;
							case 2: TryAdd(pos.x,pos.y-1); continue;
							case 3: TryAdd(pos.x,pos.y+1); continue;
						}
					}
				}
			}
		}
	}
}
