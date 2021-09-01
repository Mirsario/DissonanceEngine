using System.Collections.Generic;

namespace Dissonance.Engine.Graphics
{
	public class Font
	{
		public int Size { get; set; } = 7;
		public Texture Texture { get; private set; }
		public string CharList { get; private set; }
		public Vector2 CharSize { get; private set; }
		public Dictionary<char, Vector2[]> CharToUv { get; private set; }

		public Font(string charList, Texture texture, Vector2 charSize, int offset = 0)
		{
			Texture = texture;
			CharList = charList;
			CharSize = charSize;

			CharToUv = new Dictionary<char, Vector2[]>();

			var size = new Vector2(charSize.x / texture.Width, charSize.y / texture.Height);

			float x = 0f;
			float y = 0f;

			for (int i = 0; i < offset; i++) {
				x += size.x;

				if (x >= 1f) {
					x = 0f;
					y += size.y;
				}
			}

			for (int i = 0; i < charList.Length; i++) {
				CharToUv.Add(
					charList[i],
					new Vector2[4] {
						new Vector2(x,			y),
						new Vector2(x + size.x,	y),
						new Vector2(x + size.x,	y + size.y),
						new Vector2(x,			y + size.y)
					}
				);

				x += size.x;

				if (x >= 1f) {
					x = 0f;
					y += size.y;
				}
			}
		}
	}
}
