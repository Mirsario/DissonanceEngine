namespace AbyssCrusaders.Tiles
{
	public class Clay : SolidTileBase
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<TerrariaFrameset>();
	}
}
