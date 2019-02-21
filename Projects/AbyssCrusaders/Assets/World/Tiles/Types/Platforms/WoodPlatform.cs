namespace AbyssCrusaders.Tiles.Platforms
{
	public class WoodPlatform : PlatformTileBase
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<PlatformFrameset>();
	}
}
