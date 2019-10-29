using System.Collections.Generic;
using AbyssCrusaders.Core;
using GameEngine;

namespace AbyssCrusaders.Content.TileFramesets
{
	public class PlatformFrameset : TileFrameset
	{
		public override void OnInit()
		{
			tileTextureFrameSize = 8;
			tileTextureSize = 8;
			
			tileFramesets = new Vector2UShort[][] {
				#region Default
				TilesetHelper.CreateTileset(new Vector2UShort(0,0),new Dictionary<Vector2UShort,byte[]>() {
					{	//Free
						new Vector2UShort(0,0), 
						TilesetHelper.CalculateMasks( 
							null,	null,	null,
							false,			false,
							null,	null,	null
						)
					},
					{	//Left
						new Vector2UShort(1,0), 
						TilesetHelper.CalculateMasks( 
							null,	null,	null,
							false,			true,
							null,	null,	null
						)
					},
					{	//Center
						new Vector2UShort(2,0), 
						TilesetHelper.CalculateMasks( 
							null,	null,	null,
							true,			true,
							null,	null,	null
						)
					},
					{	//Right
						new Vector2UShort(3,0), 
						TilesetHelper.CalculateMasks( 
							null,	null,	null,
							true,			false,
							null,	null,	null
						)
					}
				})
				#endregion
			};
		}
	}
}
