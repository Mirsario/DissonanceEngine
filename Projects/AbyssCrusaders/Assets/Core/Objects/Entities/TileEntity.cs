using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace AbyssCrusaders.Core
{
	public abstract class TileEntity : Entity
	{
		public Chunk chunk;
		public ushort idInChunk;
		public Vector2Int position;
		public Vector2UShort positionInChunk;

		protected TileEntity() {}

		internal void Init(Chunk chunk,ushort idInChunk,Vector2Int position)
		{
			this.chunk = chunk;
			this.idInChunk = idInChunk;
			this.position = position;

			positionInChunk = (Vector2UShort)(position-chunk.positionInTiles);

			OnInit();
		}
	}
}
