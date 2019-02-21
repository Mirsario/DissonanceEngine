namespace AbyssCrusaders.Tiles
{
	public class Sand : SolidTileBase
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<TerrariaFrameset>();
	}
}
