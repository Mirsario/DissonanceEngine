using System.Collections.Generic;
using GameEngine;
using AbyssCrusaders.DataStructures;

namespace AbyssCrusaders.Tiles
{
	public class TallGrassFrameset : TileFrameset
	{
		public override void OnInit()
		{
			tileFramesets = new Vector2UShort[][] {
				TilesetHelper.CreateTileset(new Vector2UShort(0,0)),
				TilesetHelper.CreateTileset(new Vector2UShort(1,0)),
				TilesetHelper.CreateTileset(new Vector2UShort(2,0)),
				TilesetHelper.CreateTileset(new Vector2UShort(3,0))
			};
		}
	}
}
