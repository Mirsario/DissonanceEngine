using AbyssCrusaders.Content.PhysicMaterials;
using AbyssCrusaders.Content.TileFramesets;
using AbyssCrusaders.Content.Tiles.Common;
using AbyssCrusaders.Core;
using AbyssCrusaders.Core.DataStructures;
using GameEngine;

namespace AbyssCrusaders.Content.Tiles.Natural
{
	public class DirtTile : SolidTileBase, IHasMaterial
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<TestFrameset>();

		public override void OnInit()
		{
			base.OnInit();

			loot = new LootInfo(Item.GetTypeId<Items.Placeables.Building.Dirt>());
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector2? atPoint) => PhysicMaterial.GetMaterial<DirtPhysicMaterial>();
	}
}
