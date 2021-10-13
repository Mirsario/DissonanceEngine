using System.Collections.Generic;
using Dissonance.Engine.IO;

namespace Dissonance.Engine.Graphics
{
	public class Font
	{
		public int Size { get; set; } = 7;
		public Asset<Texture> Texture { get; private set; }
		public string CharList { get; private set; }
		public Vector2 CharSize { get; private set; }
		public Dictionary<char, Vector2[]> CharToUv { get; private set; }

		public Font(string charList, Asset<Texture> texture, Vector2 charSize, int offset = 0)
		{
			Texture = texture;
			CharList = charList;
			CharSize = charSize;

			CharToUv = new Dictionary<char, Vector2[]>();

			var textureValue = texture.GetValueImmediately();
			var size = new Vector2(charSize.X / textureValue.Width, charSize.Y / textureValue.Height);

			float x = 0f;
			float y = 0f;

			for (int i = 0; i < offset; i++) {
				x += size.X;

				if (x >= 1f) {
					x = 0f;
					y += size.Y;
				}
			}

			for (int i = 0; i < charList.Length; i++) {
				CharToUv.Add(
					charList[i],
					new Vector2[4] {
						new Vector2(x,			y),
						new Vector2(x + size.X,	y),
						new Vector2(x + size.X,	y + size.Y),
						new Vector2(x,			y + size.Y)
					}
				);

				x += size.X;

				if (x >= 1f) {
					x = 0f;
					y += size.Y;
				}
			}
		}
	}
}
