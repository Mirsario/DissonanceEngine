using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace AbyssCrusaders.Core
{
	public partial class Chunk : ISaveable
	{
		public const int ChunkSize = 64;
		public const int ChunkSizeInPixels = ChunkSize*Main.UnitSizeInPixels;

		public Tile[,] tiles = new Tile[ChunkSize,ChunkSize];
		public List<TileEntity> tileEntities = new List<TileEntity>();
		public World world;
		public Vector2Int position;
		public Vector2Int positionInTiles;

		public Chunk(World world,Vector2Int pos)
		{
			this.world = world;

			position = pos;
			positionInTiles = position*ChunkSize;

			InitRendering();
		}

		public void Save(BinaryWriter writer)
		{
			for(int y = 0;y<ChunkSize;y++) {
				for(int x = 0;x<ChunkSize;x++) {
					tiles[x,y].Save(writer);
				}
			}
		}
		public void Load(BinaryReader reader)
		{
			for(int y = 0;y<ChunkSize;y++) {
				for(int x = 0;x<ChunkSize;x++) {
					tiles[x,y].Load(reader);
				}
			}
		}

		public Vector2Int LocalPointToWorld(int x,int y) => new Vector2Int(position.x*ChunkSize+x,position.y*ChunkSize+y);
	}
}
