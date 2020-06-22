using Dissonance.Engine.Graphics.Textures;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Graphics.UserInterface
{
	public class GUIStyle
	{
		public Texture texInactive;
		public Texture texNormal;
		public Texture texHover;
		public Texture texActive;
		public RectOffset border;
		public TextAlignment textAlignment;
		public int fontSize = 16;

		public GUIStyle()
		{
			texInactive = GUI.texDefaultInactive;
			texNormal = GUI.texDefault;
			texHover = GUI.texDefaultHover;
			texActive = GUI.texDefaultActive;
			border = new RectOffset(6f,6f,6f,6f);
			textAlignment = TextAlignment.UpperLeft;
		}
	}
}

