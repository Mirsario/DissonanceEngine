using OpenTK;
using GameEngine.Graphics;

namespace GameEngine
{
	public static class Screen
	{
		internal static int width;
		internal static int height;
		internal static Vector2Int size;
		internal static Vector2 sizeFloat;
		internal static Vector2 center;
		internal static RectInt rectangle;
		internal static int windowX;
		internal static int windowY;
		internal static Vector2Int windowLocation;
		internal static Vector2 windowCenter;

		public static bool lockCursor;
		public static bool showCursor;

		public static int Width => width;
		public static int Height => height;
		public static Vector2Int Size => size;
		public static Vector2 Center => center;
		public static RectInt Rectangle => rectangle;
		public static int WindowX => Rendering.window.Location.X;
		public static int WindowY => Rendering.window.Location.Y;
		public static Vector2Int WindowLocation => windowLocation;
		public static Vector2 WindowCenter => windowCenter;

		public static bool Fullscreen {
			get => Rendering.window.WindowState==WindowState.Fullscreen;
			set {
				if(value) {
					Rendering.window.WindowState = WindowState.Fullscreen;
				} else {
					Rendering.window.WindowState = WindowState.Normal;
				}
			}
		}

		internal static void UpdateValues(GameWindow window)
		{
			width = window.Width;
			height = window.Height;
			size = new Vector2Int(width,height);
			sizeFloat = (Vector2)size;
			center = new Vector2(width*0.5f,height*0.5f);
			rectangle = new RectInt(0,0,width,height);

			windowX = Rendering.window.Location.X;
			windowY = Rendering.window.Location.Y;
			windowLocation = new Vector2Int(windowX,windowY);
			windowCenter = new Vector2(windowX+width*0.5f,windowY+height*0.5f);
		}
	}
}