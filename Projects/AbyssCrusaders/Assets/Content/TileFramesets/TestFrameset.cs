using System.Collections.Generic;
using GameEngine;
using AbyssCrusaders.DataStructures;
using AbyssCrusaders.Core;

namespace AbyssCrusaders.Content.TileFramesets
{
	public class TestFrameset : TileFrameset
	{
		public override void OnInit()
		{
			tileTextureFrameSize = 10;
			tileTextureSize = 10;
			
			//Tile Frameset
			tileFramesets = new Vector2UShort[][] {
				#region Default
				TilesetHelper.CreateTileset(new Vector2UShort(1,1),new Dictionary<Vector2UShort,byte[]>() {
					#region Corners
					{	//TopLeftCorner
						new Vector2UShort(0,0),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							false,			true,
							null,	true,	true
						)
					},
					{	//TopCorner
						new Vector2UShort(1,0),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			true,
							true,	true,	true
						)
					},
					{	//TopRightCorner
						new Vector2UShort(2,0),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			false,
							true,	true,	null
						)
					},
					{	//RightCorner
						new Vector2UShort(2,1),
						TilesetHelper.CalculateMasks(
							true,	true,	null,
							true,			false,
							true,	true,	null
						)
					},
					{	//BottomRightCorner
						new Vector2UShort(2,2),
						TilesetHelper.CalculateMasks(
							true,	true,	null,
							true,			false,
							null,	false,	null
						)
					},
					{	//BottomCorner
						new Vector2UShort(1,2),
						TilesetHelper.CalculateMasks(
							true,	true,	true,
							true,			true,
							null,	false,	null
						)
					},
					{	//BottomLeftCorner
						new Vector2UShort(0,2),
						TilesetHelper.CalculateMasks(
							null,	true,	true,
							false,			true,
							null,	false,	null
						)
					},
					{	//LeftCorner
						new Vector2UShort(0,1),
						TilesetHelper.CalculateMasks(
							null,	true,	true,
							false,			true,
							null,	true,	true
						)
					},
					#endregion
				})
				#endregion
			};

			//Wall Frameset
			wallFrameset = TilesetHelper.CreateWallset(
				new WallDrawInfo(0,5,1,1,-1,-1),	new WallDrawInfo(1,5,1,1, 0,-1),		new WallDrawInfo(2,5,1,1, 1,-1),
				new WallDrawInfo(0,6,1,1,-1, 0),	new WallDrawInfo(1,6,1,1, 0, 0),		new WallDrawInfo(2,6,1,1, 1, 0),
				new WallDrawInfo(0,7,1,1,-1, 1),	new WallDrawInfo(1,7,1,1, 0, 1),		new WallDrawInfo(2,7,1,1, 1, 1)
			);
		}
	}
}
