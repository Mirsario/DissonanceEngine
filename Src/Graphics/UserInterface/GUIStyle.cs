namespace Dissonance.Engine.Graphics
{
	public class GUIStyle
	{
		public Texture TexInactive { get; set; }
		public Texture TexNormal { get; set; }
		public Texture TexHover { get; set; }
		public Texture TexActive { get; set; }
		public RectOffset Border { get; set; }
		public TextAlignment TextAlignment { get; set; }
		public int FontSize { get; set; } = 16;

		public GUIStyle()
		{
			TexInactive = GUI.TexDefaultInactive;
			TexNormal = GUI.TexDefault;
			TexHover = GUI.TexDefaultHover;
			TexActive = GUI.TexDefaultActive;
			Border = new RectOffset(6f, 6f, 6f, 6f);
			TextAlignment = TextAlignment.UpperLeft;
		}
	}
}

