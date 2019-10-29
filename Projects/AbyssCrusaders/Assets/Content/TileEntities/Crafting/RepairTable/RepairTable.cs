using AbyssCrusaders.Content.TileFramesets;
using AbyssCrusaders.Core;
using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders.TileEntities.Crafting
{
	public class RepairTable : TileEntity
	{
		public override void OnInit()
		{
			base.OnInit();

			var sprite = AddComponent<Sprite>(c => {
				c.Material = Resources.Get<Material>("RepairTable.material");
			});
		}
	}
}
