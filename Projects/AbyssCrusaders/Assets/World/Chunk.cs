using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace AbyssCrusaders
{
	public partial class Chunk : ISaveable
	{
		public const int ChunkSize = 64;
		public const int ChunkSizeInPixels = ChunkSize*Main.UnitSizeInPixels;
		
		public Vector2Int position;
		public Vector2Int positionInTiles;
		public World world;
		public Tile[,] tiles;

		public Chunk(World world,Vector2Int pos)
		{
			position = pos;
			positionInTiles = position*ChunkSize;
			this.world = world;

			tiles = new Tile[ChunkSize,ChunkSize];

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
