using AbyssCrusaders.DataStructures;

namespace AbyssCrusaders
{
	public abstract partial class World : GameObject2D
	{
		public void TileFrame(int x,int y)
		{
			ref var tile = ref GetTileWithChunk(x,y,out Chunk chunk);

			chunk.updateTexture = true;
			
			tile.tileFrame = 0;
			tile.wallFrame = 0;

			if(tile.type==0 && tile.wall==0) {
				return;
			}
				
			bool cantGoLeft = x-1<0;
			bool cantGoUp = y-1<0;
			bool cantGoRight = x+1>=width;
			bool cantGoDown = y+1>=height;

			if(tile.type!=0) {
				var tilePreset = tile.TilePreset;

				if(tilePreset.frameset!=null && tilePreset.frameset.tileFramesets!=null) {
					int numFramesets = tilePreset.frameset.tileFramesets.Length;
					if(tile.style>numFramesets) {
						tile.style = (byte)(numFramesets-1);
					}
				}

				//redesign?
				bool TileCheck(Tile thisTile,Tile otherTile) {
					return otherTile.type>0 && (thisTile.type==otherTile.type || tilePreset.BlendsWithTile(thisTile,otherTile) || otherTile.TilePreset.BlendsWithTile(otherTile,thisTile));
				}
				
				tile.tileFrame = new BitsByte(
					cantGoLeft || cantGoUp || TileCheck(tile,this[x-1,y-1]),	//Top Left
					cantGoUp || TileCheck(tile,this[x,y-1]),					//Top
					cantGoRight || cantGoUp || TileCheck(tile,this[x+1,y-1]),	//Top Right
					cantGoLeft || TileCheck(tile,this[x-1,y]),					//Left
					cantGoRight || TileCheck(tile,this[x+1,y]),					//Right
					cantGoLeft || cantGoDown || TileCheck(tile,this[x-1,y+1]),	//Bottom Left
					cantGoDown || TileCheck(tile,this[x,y+1]),					//Bottom
					cantGoRight || cantGoDown || TileCheck(tile,this[x+1,y+1])	//Bottom Right
				);
			}

			if(tile.wall!=0) {
				bool WallCheck(Tile thisTile,Tile otherTile,bool corner) {
					return otherTile.wall==thisTile.wall; //return corner ? otherTile.wall==thisTile.wall : otherTile.wall!=0;
				}

				tile.wallFrame = new BitsByte(
					cantGoLeft || cantGoUp || WallCheck(tile,this[x-1,y-1],true),	//Top Left
					cantGoUp || WallCheck(tile,this[x,y-1],false),					//Top
					cantGoRight || cantGoUp || WallCheck(tile,this[x+1,y-1],true),	//Top Right
					cantGoLeft || WallCheck(tile,this[x-1,y],false),				//Left
					cantGoRight || WallCheck(tile,this[x+1,y],false),				//Right
					cantGoLeft || cantGoDown || WallCheck(tile,this[x-1,y+1],true),	//Bottom Left
					cantGoDown || WallCheck(tile,this[x,y+1],false),				//Bottom
					cantGoRight || cantGoDown || WallCheck(tile,this[x+1,y+1],true)	//Bottom Right
				);
			}
		}
		public void DiamondTileFrame(int x,int y)
		{
			TileFrame(x-1,y);
			TileFrame(x+1,y);
			TileFrame(x,y-1);
			TileFrame(x,y+1);
		}
		public void SquareTileFrame(int x,int y)
		{
			TileFrame(x-1,y-1);	TileFrame(x,y-1);	TileFrame(x+1,y-1);
			TileFrame(x-1,y);	TileFrame(x,y);		TileFrame(x+1,y);
			TileFrame(x-1,y+1);	TileFrame(x,y+1);	TileFrame(x+1,y+1);
		}

		public void RemoveTile(int x,int y)
		{
			ref var tile = ref GetTileWithChunk(x,y,out Chunk chunk);

			if(tile.type!=0) {
				tile.type = 0;
				SquareTileFrame(x,y);
			}
		}
		public void RemoveWall(int x,int y)
		{
			ref var tile = ref GetTileWithChunk(x,y,out Chunk chunk);

			if(tile.wall!=0) {
				tile.wall = 0;
				SquareTileFrame(x,y);
			}
		}
		public void PlaceTile(int x,int y,ushort type)
		{
			ref var tile = ref GetTileWithChunk(x,y,out Chunk chunk);

			if(tile.type!=type) {
				tile.type = type;
				SquareTileFrame(x,y);
			}
		}
		public void PlaceWall(int x,int y,ushort type)
		{
			if(type>=TilePreset.typeCount) {
				return;
			}

			ref var tile = ref GetTileWithChunk(x,y,out Chunk chunk);

			if(tile.wall!=type) {
				var preset = TilePreset.byId[type];
				if(!preset.canBeWall) {
					return;
				}

				tile.wall = type;
				SquareTileFrame(x,y);
			}
		}
	}
}
