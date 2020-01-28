namespace GameEngine
{
	public class GUISkin
	{
		public GUIStyle boxStyle;
		public GUIStyle buttonStyle;
		public GUISkin()
		{
			boxStyle = new GUIStyle();
			buttonStyle = new GUIStyle {
				textAlignment = TextAlignment.MiddleCenter
			};
		}
	}
}

