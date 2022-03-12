namespace Dissonance.Engine.Graphics
{
	public class GUISkin
	{
		public GUIStyle BoxStyle { get; set; }
		public GUIStyle ButtonStyle { get; set; }

		public GUISkin()
		{
			BoxStyle = new GUIStyle();
			ButtonStyle = new GUIStyle {
				TextAlignment = TextAlignment.MiddleCenter
			};
		}
	}
}

