using AbyssCrusaders.Content.TileFramesets;
using AbyssCrusaders.Core;

namespace AbyssCrusaders.Tiles
{
	public class TallGrass : TilePreset
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<TallGrassFrameset>();

		public override void OnInit()
		{
			transparent = true;
		}
	}
}
