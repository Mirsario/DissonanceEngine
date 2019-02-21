using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders.Tiles
{
	public abstract class SolidTileBase : TilePreset
	{
		public override Texture Texture => Resources.Get<Texture>($"{GetType().Name}.png");

		public override void OnInit()
		{
			collision = CollisionInfo.Full;
			canBeWall = true;
		}
	}
}
