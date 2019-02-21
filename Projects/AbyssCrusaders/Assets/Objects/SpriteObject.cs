using GameEngine;

namespace AbyssCrusaders
{
	public class SpriteObject : GameObject2D
	{
		public Sprite sprite;

		public override void OnInit()
		{
			sprite = AddComponent<Sprite>();
		}
	}
}
