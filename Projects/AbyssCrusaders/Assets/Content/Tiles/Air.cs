using AbyssCrusaders.Core;
using GameEngine.Graphics;

namespace AbyssCrusaders.Content.Tiles
{
	[ForceID(0)]
	public class Air : TilePreset
	{
		public override Texture Texture => null;

		protected override TileFrameset Frameset => null;

		public override void OnInit()
		{
			transparent = true;
		}
	}
}
