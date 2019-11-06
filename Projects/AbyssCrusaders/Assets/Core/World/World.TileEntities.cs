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

			var entity = Entity.Instantiate<T>(this,position:(Vector2)position,init:false);

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

			entity.PreInit(chunk,idInChunk.Value,new Vector2Int(x,y));
			entity.Init();
		}
	}
}
