using Dissonance.Engine.IO;

namespace Dissonance.Engine.Graphics;

public class GUIStyle
{
	public Asset<Texture> TexInactive { get; set; }
	public Asset<Texture> TexNormal { get; set; }
	public Asset<Texture> TexHover { get; set; }
	public Asset<Texture> TexActive { get; set; }
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

