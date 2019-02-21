using System.Collections.Generic;
using GameEngine;
using AbyssCrusaders.DataStructures;

namespace AbyssCrusaders.Tiles
{
	public class DefaultFrameset : TileFrameset
	{
		public override void OnInit()
		{
			tileTextureSize = 8;
			tileTextureFrameSize = 8;
			
			//Tile Frameset
			tileFramesets = new Vector2UShort[][] {
				#region Default
				TilesetHelper.CreateTileset(new Vector2UShort(6,3),new Dictionary<Vector2UShort,byte[]>() {
					#region Basic
					{	//Free
						new Vector2UShort(3,3), 
						TilesetHelper.CalculateMasks( 
							null,	false,	null,
							false,			false,
							null,	false,	null
						)
					},
					{	//Cross
						new Vector2UShort(9,1),
						TilesetHelper.CalculateMasks(
							null,	true,	null,
							true,			true,
							null,	true,	null
						)
					},
					{	//Surrounded
						new Vector2UShort(1,1),
						TilesetHelper.CalculateMasks(
							true,	true,	true,
							true,			true,
							true,	true,	true
						)
					},
					#endregion
					
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
					
					#region VerticalLine
					{	//VerticalTop
						new Vector2UShort(3,0),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							false,			false,
							null,	true,	null
						)
					},
					{	//VerticalMiddle
						new Vector2UShort(3,1),
						TilesetHelper.CalculateMasks(
							null,	true,	null,
							false,			false,
							null,	true,	null
						)
					},
					{	//VerticalBottom
						new Vector2UShort(3,2),
						TilesetHelper.CalculateMasks(
							null,	true,	null,
							false,			false,
							null,	false,	null
						)
					},
					#endregion
					#region HorizontalLine
					{	//HorizontalLeft
						new Vector2UShort(0,3),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							false,			true,
							null,	false,	null
						)
					},
					{	//HorizontalMiddle
						new Vector2UShort(1,3),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			true,
							null,	false,	null
						)
					},
					{	//HorizontalRight
						new Vector2UShort(2,3),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			false,
							null,	false,	null
						)
					},
					#endregion
					
					#region ElbowsAndTees
					{	//ElbowTopLeft
						new Vector2UShort(8,0),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							false,			true,
							null,	true,	false
						)
					},
					{	//TeeTop
						new Vector2UShort(9,0),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			true,
							false,	true,	false
						)
					},
					{	//ElbowTopRight
						new Vector2UShort(10,0),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			false,
							false,	true,	null
						)
					},
					{	//TeeRight
						new Vector2UShort(10,1),
						TilesetHelper.CalculateMasks(
							false,	true,	null,
							true,			false,
							false,	true,	null
						)
					},
					{	//ElbowBottomRight
						new Vector2UShort(10,2),
						TilesetHelper.CalculateMasks(
							false,	true,	null,
							true,			false,
							null,	false,	null
						)
					},
					{	//TeeBottom
						new Vector2UShort(9,2),
						TilesetHelper.CalculateMasks(
							false,	true,	false,
							true,			true,
							null,	false,	null
						)
					},
					{	//ElbowBottomLeft
						new Vector2UShort(8,2),
						TilesetHelper.CalculateMasks(
							null,	true,	false,
							false,			true,
							null,	false,	null
						)
					},
					{	//TeeLeft
						new Vector2UShort(8,1),
						TilesetHelper.CalculateMasks(
							null,	true,	false,
							false,			true,
							null,	true,	false
						)
					},
					#endregion
					#region ConnectedElbowsAndTees
					{	//ConnectedElbowTopLeft
						new Vector2UShort(11,0),
						TilesetHelper.CalculateMasks(
							true,	true,	true,
							true,			true,
							true,	true,	false
						)
					},
					{	//ConnectedTeeTop
						new Vector2UShort(12,0),
						TilesetHelper.CalculateMasks(
							true,	true,	true,
							true,			true,
							false,	true,	false
						)
					},
					{	//ConnectedElbowTopRight
						new Vector2UShort(13,0),
						TilesetHelper.CalculateMasks(
							true,	true,	true,
							true,			true,
							false,	true,	true
						)
					},
					{	//ConnectedTeeRight
						new Vector2UShort(13,1),
						TilesetHelper.CalculateMasks(
							false,	true,	true,
							true,			true,
							false,	true,	true
						)
					},
					{	//ConnectedElbowBottomLeft
						new Vector2UShort(13,2),
						TilesetHelper.CalculateMasks(
							false,	true,	true,
							true,			true,
							true,	true,	true
						)
					},
					{	//ConnectedTeeBottom
						new Vector2UShort(12,2),
						TilesetHelper.CalculateMasks(
							false,	true,	false,
							true,			true,
							true,	true,	true
						)
					},
					{	//ConnectedElbowBottomRight
						new Vector2UShort(11,2),
						TilesetHelper.CalculateMasks(
							true,	true,	false,
							true,			true,
							true,	true,	true
						)
					},
					{	//ConnectedTeeLeft
						new Vector2UShort(11,1),
						TilesetHelper.CalculateMasks(
							true,	true,	false,
							true,			true,
							true,	true,	false
						)
					},
					#endregion

					#region Diagonal
					{	//DiagonalA
						new Vector2UShort(6,2),
						TilesetHelper.CalculateMasks(
							true,	true,	false,
							true,			true,
							false,	true,	true
						)
					},
					{	//DiagonalB
						new Vector2UShort(7,2),
						TilesetHelper.CalculateMasks(
							false,	true,	true,
							true,			true,
							true,	true,	false
						)
					},
					#endregion

					#region FiveCorner
					//How else would I call this?
					{	//Corner5VerticalA
						new Vector2UShort(4,0),
						TilesetHelper.CalculateMasks(
							null,	true,	true,
							false,			true,
							null,	true,	false
						)
					},
					{	//Corner5VerticalB
						new Vector2UShort(5,0),
						TilesetHelper.CalculateMasks(
							true,	true,	null,
							true,			false,
							false,	true,	null
						)
					},
					{	//Corner5VerticalC
						new Vector2UShort(4,1),
						TilesetHelper.CalculateMasks(
							null,	true,	false,
							false,			true,
							null,	true,	true
						)
					},
					{	//Corner5VerticalD
						new Vector2UShort(5,1),
						TilesetHelper.CalculateMasks(
							false,	true,	null,
							true,			false,
							true,	true,	null
						)
					},
					
					{	//Corner5HorizontalA
						new Vector2UShort(4,2),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			true,
							true,	true,	false
						)
					},
					{	//Corner5HorizontalB
						new Vector2UShort(5,2),
						TilesetHelper.CalculateMasks(
							null,	false,	null,
							true,			true,
							false,	true,	true
						)
					},
					{	//Corner5HorizontalC
						new Vector2UShort(4,3),
						TilesetHelper.CalculateMasks(
							true,	true,	false,
							true,			true,
							null,	false,	null
						)
					},
					{	//Corner5HorizontalD
						new Vector2UShort(5,3),
						TilesetHelper.CalculateMasks(
							false,	true,	true,
							true,			true,
							null,	false,	null
						)
					}
					#endregion
				})
				#endregion
			};

			//Wall Frameset
			wallFrameset = TilesetHelper.CreateWallset(
				new WallDrawInfo(0,5,1,1,-1,-1),	new WallDrawInfo(1,5,1,1, 0,-1),		new WallDrawInfo(2,5,1,1, 1,-1),
				new WallDrawInfo(0,6,1,1,-1, 0),	new WallDrawInfo(1,6,1,1, 0, 0),		new WallDrawInfo(2,6,1,1, 1, 0),
				new WallDrawInfo(0,7,1,1,-1, 1),	new WallDrawInfo(1,7,1,1, 0, 1),		new WallDrawInfo(2,7,1,1, 1, 1),
				new Dictionary<byte[],WallDrawInfo[]> {
					{	//Completely Free
						TilesetHelper.CalculateMasks(
							false,	false,	false,
							false,			false,
							false,	false,	false
						),
						new[] { new WallDrawInfo(0,5,3,3,-1,-1) }
					}
				}
			);
		}
	}
}
