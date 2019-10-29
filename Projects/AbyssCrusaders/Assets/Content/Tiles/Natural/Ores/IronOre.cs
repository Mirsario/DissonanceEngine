using AbyssCrusaders.Content.PhysicMaterials;
using AbyssCrusaders.Content.TileFramesets;
using AbyssCrusaders.Content.Tiles.Common;
using AbyssCrusaders.Core;
using GameEngine;

namespace AbyssCrusaders.Content.Tiles.Natural.Ores
{
	public class IronOre : SolidTileBase, IHasMaterial
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<TerrariaFrameset>();

		public override void OnInit()
		{
			base.OnInit();

			canBeWall = false;
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector2? atPoint) => PhysicMaterial.GetMaterial<MetalPhysicMaterial>();
	}
}
