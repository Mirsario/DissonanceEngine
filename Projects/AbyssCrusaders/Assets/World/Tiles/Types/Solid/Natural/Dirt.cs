namespace AbyssCrusaders.Tiles
{
	public class Dirt : SolidTileBase
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<TestFrameset>();
	}
}
