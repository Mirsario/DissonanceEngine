using AbyssCrusaders.Content.PhysicMaterials;
using AbyssCrusaders.Content.TileFramesets;
using AbyssCrusaders.Content.Tiles.Common;
using AbyssCrusaders.Core;
using AbyssCrusaders.Core.DataStructures;
using GameEngine;

namespace AbyssCrusaders.Content.Tiles.Building
{
	public class WoodTile : SolidTileBase, IHasMaterial
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<DefaultFrameset>();

		public override void OnInit()
		{
			base.OnInit();

			loot = new LootInfo(Item.GetTypeId<Items.Placeables.Building.Wood>());
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector2? atPoint) => PhysicMaterial.GetMaterial<WoodPhysicMaterial>();
	}
}
