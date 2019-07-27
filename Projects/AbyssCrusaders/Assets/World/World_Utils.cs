using GameEngine;

namespace AbyssCrusaders
{
	partial class World
	{
		public Chunk GetChunkAt(int x,int y)
		{
			x = Mathf.Repeat(x,width);
			y = Mathf.Repeat(y,height);
			int chunkX = x/Chunk.ChunkSize;
			int chunkY = y/Chunk.ChunkSize;
			ref var chunk = ref chunks[chunkX,chunkY];
			if(chunk==null) {
				chunk = new Chunk(this,new Vector2Int(chunkX,chunkY));
			}
			return chunk;
		}
		public ref Tile GetTileWithChunk(int x,int y,out Chunk chunk)
		{
			x = Mathf.Repeat(x,width);
			y = Mathf.Repeat(y,height);
			int chunkX = x/Chunk.ChunkSize;
			int chunkY = y/Chunk.ChunkSize;
			chunk = chunks[chunkX,chunkY];
			if(chunk==null) {
				chunks[chunkX,chunkY] = chunk = new Chunk(this,new Vector2Int(chunkX,chunkY));
			}
			return ref chunk.tiles[x%Chunk.ChunkSize,y%Chunk.ChunkSize];
		}
		public int ClampX(int x) => x<0 ? 0 : (x>=width ? width-1 : x);
		public int ClampY(int y) => y<0 ? 0 : (y>=height ? height-1 : y);
		public Vector2Int Clamp(Vector2Int vector)
		{
			if(vector.x<0) {
				vector.x = 0;
			}else if(vector.x>=width) {
				vector.x = width-1;
			}
			if(vector.y<0) {
				vector.y = 0;
			}else if(vector.y>=height) {
				vector.y = height-1;
			}
			return vector;
		}
	}
}
