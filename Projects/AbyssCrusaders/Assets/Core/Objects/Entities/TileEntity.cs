using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;
using System;
using System.Linq;
using System.Reflection;

namespace AbyssCrusaders.Core
{
	public abstract class TileEntity : Entity
	{
		public Chunk chunk;
		public ushort idInChunk;
		public Vector2Int position;
		public Vector2UShort positionInChunk;
		public Vector2UShort sizeInTiles = new Vector2UShort(1,1);

		public ushort Id { get; private set; }

		protected TileEntity() {}

		internal void PreInit(Chunk chunk,ushort idInChunk,Vector2Int position)
		{
			this.chunk = chunk;
			this.idInChunk = idInChunk;
			this.position = position;

			positionInChunk = (Vector2UShort)(position-chunk.positionInTiles);
		}
	}
}
