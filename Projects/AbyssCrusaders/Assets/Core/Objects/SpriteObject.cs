using GameEngine;
using GameEngine.Graphics;
using System;

namespace AbyssCrusaders.Core
{
	public class SpriteObject : GameObject2D
	{
		public Sprite sprite;

		private Vector2Int frameSize;
		private Texture texture;

		public Texture Texture {
			get => texture;
			set {
				sprite.Material.SetTexture("mainTex",texture = value);

				if(texture!=null) {
					Transform.LocalScale = new Vector3(frameSize.x*Main.PixelSizeInUnits,frameSize.y*Main.PixelSizeInUnits,1f);
				}
			}
		}
		public Vector2Int FrameSize {
			get => frameSize;
			set {
				if(value.x<=0 || value.y<=0) {
					throw new ArgumentException("Values must not be equal to or less than zero.");
				}

				frameSize = value;
			}
		}

		public override void OnInit()
		{
			sprite = AddComponent<Sprite>();
		}

		public static SpriteObject Create(GameObject parentObject,string textureName,Material material,Vector2Int? frameSize = null) => Create(parentObject,Resources.Get<Texture>(textureName),material,frameSize);
		public static SpriteObject Create(GameObject parentObject,Texture texture,Material material,Vector2Int? frameSize = null)
		{
			var spriteObj = Instantiate<SpriteObject>();

			spriteObj.frameSize = frameSize ?? texture.Size;
			spriteObj.sprite.Material = material;
			spriteObj.Transform.parent = parentObject.Transform;
			spriteObj.Texture = texture;

			return spriteObj;
		}
	}
}
