using AbyssCrusaders.Content.PhysicMaterials;
using AbyssCrusaders.Content.TileFramesets;
using AbyssCrusaders.Content.Tiles.Common;
using AbyssCrusaders.Core;
using GameEngine;

namespace AbyssCrusaders.Content.Tiles.Building.Platforms
{
	public class WoodPlatform : PlatformTileBase, IHasMaterial
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<PlatformFrameset>();

		PhysicMaterial IHasMaterial.GetMaterial(Vector2? atPoint) => PhysicMaterial.GetMaterial<WoodPhysicMaterial>();
	}
}
