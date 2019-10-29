using AbyssCrusaders.DataStructures;
using GameEngine;
using System;

namespace AbyssCrusaders.Core
{
	partial class World
	{
		public void PlaceTileEntity<T>(int x,int y) where T : TileEntity
		{
			var chunk = GetChunkAt(x,y);

			var position = new Vector2Int(x,y);

			var entity = Entity.Instantiate<T>(this,position:(Vector2)position);

			ushort? idInChunk = null;

			for(int i = 0;i<chunk.tileEntities.Count;i++) {
				if(chunk.tileEntities[i]==null) {
					idInChunk = (ushort)i;

					chunk.tileEntities[i] = entity;
				}
			}

			if(!idInChunk.HasValue) {
				idInChunk = (ushort)chunk.tileEntities.Count;

				chunk.tileEntities.Add(entity);
			}

			entity.Init(chunk,idInChunk.Value,new Vector2Int(x,y));

			/*ref var tile = ref this[x,y];

			if(tile.type==type) {
				return;
			}

			tile.type = type;
			tile.tileDamage = 0;

			if(Netplay.isClient && playSound) {
				tile.PlayTileSound(x,y,"Place");
			}

			SquareTileFrame(x,y);*/
		}
	}
}
