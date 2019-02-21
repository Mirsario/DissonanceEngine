namespace AbyssCrusaders.Tiles
{
	public class CopperOre : SolidTileBase
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<TerrariaFrameset>();

		public override void OnInit()
		{
			canBeWall = false;
		}
	}
}
