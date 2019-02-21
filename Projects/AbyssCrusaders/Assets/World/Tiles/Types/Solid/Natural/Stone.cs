namespace AbyssCrusaders.Tiles
{
	public class Stone : SolidTileBase
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<TerrariaFrameset>();

		public override bool BlendsWithTile(Tile thisTile,Tile otherTile)
			=> otherTile.type==GetTypeId<Dirt>()
			|| otherTile.type==GetTypeId<Grass>();
	}
}
