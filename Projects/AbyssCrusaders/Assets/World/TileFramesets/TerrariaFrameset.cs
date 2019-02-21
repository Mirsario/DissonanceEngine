using System.Collections.Generic;
using GameEngine;
using AbyssCrusaders.DataStructures;

namespace AbyssCrusaders.Tiles
{
	public class TerrariaFrameset : TileFrameset
	{
		public override void OnInit()
		{
			tileTextureSize = 8;
			tileTextureFrameSize = 9;
			
			tileFramesets = new[] {
				TilesetHelper.CreateTileset(new Vector2UShort(6,3),new Dictionary<Vector2UShort,byte[]>() {
					#region Basic
					{	//Free
						new Vector2UShort(9,3), //+[10,3],[11,3]
						TilesetHelper.CalculateMasks( 
							null,	false,	null,
							false,			false,
							null,	false,	null
						)
					},
					{	//Surrounded
						new Vector2UShort(1,1), //+[2,1],[3,1]
						TilesetHelper.CalculateMasks(
							null,	true,	null,
							true,			true,
							null,	true,	null
						)
					},
					#endregion
					
					#region Corners
					{	//TopLeftCorner
						new Vector2UShort(0,3), //+[2,3],[4,3]
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							false,			true,
							null,	true,	null
						)
					},
					{	//TopCorner
						new Vector2UShort(1,0), //+[2,0],[3,0]
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			true,
							null,	true,	null
						)
					},
					{	//TopRightCorner
						new Vector2UShort(1,3), //+[3,3],[5,3]
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			false,
							null,	true,	null
						)
					},
					{	//RightCorner
						new Vector2UShort(4,0), //+[4,1],[4,2]
						TilesetHelper.CalculateMasks(
							null,	true,	null,
							true,			false,
							null,	true,	null
						)
					},
					{	//BottomRightCorner
						new Vector2UShort(1,4), //+[3,4],[5,4]
						TilesetHelper.CalculateMasks(
							null,	true,	null,
							true,			false,
							null,	false,	null
						)
					},
					{	//BottomCorner
						new Vector2UShort(1,2), //+[2,2],[3,2]
						TilesetHelper.CalculateMasks(
							null,	true,	null,
							true,			true,
							null,	false,	null
						)
					},
					{	//BottomLeftCorner
						new Vector2UShort(0,4), //+[2,4],[4,4]
						TilesetHelper.CalculateMasks(
							null,	true,	null,
							false,			true,
							null,	false,	null
						)
					},
					{	//LeftCorner
						new Vector2UShort(0,0), //+[0,1],[0,2]
						TilesetHelper.CalculateMasks(
							null,	true,	null,
							false,			true,
							null,	true,	null
						)
					},
					#endregion
					
					#region VerticalLine
					{	//VerticalTop
						new Vector2UShort(6,0), //+[7,0],[8,0]
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							false,			false,
							null,	true,	null
						)
					},
					{	//VerticalMiddle
						new Vector2UShort(5,0), //+[5,1],[5,2]
						TilesetHelper.CalculateMasks(
							null,	true,	null,
							false,			false,
							null,	true,	null
						)
					},
					{	//VerticalBottom
						new Vector2UShort(6,3), //+[7,3],[8,3]
						TilesetHelper.CalculateMasks(
							null,	true,	null,
							false,			false,
							null,	false,	null
						)
					},
					#endregion
					#region HorizontalLine
					{	//HorizontalLeft
						new Vector2UShort(9,0), //+[9,1],[9,2]
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							false,			true,
							null,	false,	null
						)
					},
					{	//HorizontalMiddle
						new Vector2UShort(6,4), //+[7,4],[8,4]
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			true,
							null,	false,	null
						)
					},
					{	//HorizontalRight
						new Vector2UShort(12,0), //+[12,1],[12,2]
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			false,
							null,	false,	null
						)
					},
					#endregion
				})
			};
			
			//Wall Frameset
			wallFrameset = TilesetHelper.CreateWallset(
				new WallDrawInfo(7,12,1,1,-1,-1),	new WallDrawInfo(8,12,1,1, 0,-1),		new WallDrawInfo(9,12,1,1, 1,-1),
				new WallDrawInfo(7,13,1,1,-1, 0),	new WallDrawInfo(8,13,1,1, 0, 0),		new WallDrawInfo(9,13,1,1, 1, 0),
				new WallDrawInfo(7,14,1,1,-1, 1),	new WallDrawInfo(8,14,1,1, 0, 1),		new WallDrawInfo(9,14,1,1, 1, 1)
			);
		}
	}
}
