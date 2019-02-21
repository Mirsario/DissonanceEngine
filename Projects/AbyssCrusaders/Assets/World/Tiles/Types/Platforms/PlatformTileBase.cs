using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders.Tiles.Platforms
{
	public abstract class PlatformTileBase : TilePreset
	{
		public override Texture Texture => Resources.Get<Texture>($"{GetType().Name}.png");

		public override void OnInit()
		{
			canBeWall = false;
			allowDroppingThrough = true;
			collision = new CollisionInfo {
				up = true
			};
		}
	}
}
