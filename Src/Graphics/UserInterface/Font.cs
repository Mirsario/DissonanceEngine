using System.Collections.Generic;
using Dissonance.Engine.Graphics.Textures;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Graphics.UserInterface
{
	public class Font
	{
		public Texture texture;
		public Dictionary<char,Vector2[]> charToUv;
		public string charList;
		public Vector2 charSize;
		public int size { get; set; } = 7;

		public Font(string charList,Texture texture,Vector2 charSize,int offset = 0)
		{
			this.texture = texture;
			this.charList = charList;
			this.charSize = charSize;
			charToUv = new Dictionary<char,Vector2[]>();
			var size = new Vector2(charSize.x/texture.Width,charSize.y/texture.Height);
			float x = 0f;
			float y = 0f;
			for(int i = 0;i<offset;i++) {
				x += size.x;
				if(x>=1f) {
					x = 0f;
					y += size.y;
				}
			}
			for(int i = 0;i<charList.Length;i++) {
				charToUv.Add(charList[i],new Vector2[4] {
					new Vector2(x,          y),
					new Vector2(x+size.x,   y),
					new Vector2(x+size.x,   y+size.y),
					new Vector2(x,          y+size.y)
				});

				x += size.x;
				if(x>=1f) {
					x = 0f;
					y += size.y;
				}
			}
		}
	}
}