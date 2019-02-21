namespace AbyssCrusaders.Tiles
{
	public class Wood : SolidTileBase
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<DefaultFrameset>();
	}
}
