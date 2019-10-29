using AbyssCrusaders.Content.PhysicMaterials;
using AbyssCrusaders.Content.TileFramesets;
using AbyssCrusaders.Content.Tiles.Common;
using AbyssCrusaders.Core;
using GameEngine;

namespace AbyssCrusaders.Content.Tiles.Natural
{
	public class Stone : SolidTileBase, IHasMaterial
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<TestFrameset>();

		public override bool BlendsWithTile(Tile thisTile,Tile otherTile) => otherTile.type==GetTypeId<Dirt>() || otherTile.type==GetTypeId<Grass>();

		PhysicMaterial IHasMaterial.GetMaterial(Vector2? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}
