namespace AbyssCrusaders.Tiles
{
	public class Grass : SolidTileBase
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<TestFrameset>();

		public override bool BlendsWithTile(Tile thisTile,Tile otherTile) => otherTile.type==GetTypeId<Dirt>();
	}
}
