using AbyssCrusaders.Content.PhysicMaterials;
using AbyssCrusaders.Content.TileFramesets;
using AbyssCrusaders.Content.Tiles.Common;
using AbyssCrusaders.Core;
using GameEngine;

namespace AbyssCrusaders.Content.Tiles.Natural
{
	public class Grass : SolidTileBase, IHasMaterial
	{
		protected override TileFrameset Frameset => TileFrameset.GetInstance<DefaultFrameset>();

		public override bool BlendsWithTile(Tile thisTile,Tile otherTile) => otherTile.type==GetTypeId<DirtTile>();
		public override void OnDestroyed(World world,int x,int y,bool wall)
		{
			base.OnDestroyed(world,x,y,wall);

			world[x,y].type = GetTypeId<DirtTile>();

			world.TileFrame(x,y);
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector2? atPoint) => PhysicMaterial.GetMaterial<DirtPhysicMaterial>();
	}
}
