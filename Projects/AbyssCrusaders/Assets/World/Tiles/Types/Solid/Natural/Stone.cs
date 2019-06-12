namespace AbyssCrusaders.Tiles
{
	public class Stone : SolidTileBase
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<TestFrameset>();

		public override bool BlendsWithTile(Tile thisTile,Tile otherTile)
			=> otherTile.type==GetTypeId<Dirt>()
			|| otherTile.type==GetTypeId<Grass>();
	}
}
