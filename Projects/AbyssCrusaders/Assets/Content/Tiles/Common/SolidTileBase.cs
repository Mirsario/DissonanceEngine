using AbyssCrusaders.Core;
using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders.Content.Tiles.Common
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
