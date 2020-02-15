using Dissonance.Framework.Windowing;

namespace Dissonance.Engine
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
		public static int WindowX => windowX;
		public static int WindowY => windowY;
		public static Vector2Int WindowLocation => windowLocation;
		public static Vector2 WindowCenter => windowCenter;

		/*public static bool Fullscreen {
			get => GLFW.GetWindowMonitor(Rendering.window)!=IntPtr.Zero;
			set {
				var monitor = GLFW.GetWindowMonitor(Rendering.window);
				bool isFullscreen = monitor!=IntPtr.Zero;

				if(value!=isFullscreen) {
					return;
				}

				if(value) {
					GLFW.SetWindowMonitor(Rendering.window,GLFW.GetPrimaryMonitor(),0,0,800,600,144);
				} else {
					var videoMode = GLFW.GetVideoMode(monitor);

					GLFW.SetWindowMonitor(Rendering.window,IntPtr.Zero,0,0,videoMode.width,videoMode.height,videoMode.refreshRate);
				}
			}
		}*/

		public static bool CursorVisible {
			set {
				GLFW.SetInputMode(Game.window,GLFW.CURSOR,value ? GLFW.CURSOR_NORMAL : GLFW.CURSOR_HIDDEN);
			}
		}

		internal static void UpdateValues()
		{
			GLFW.GetFramebufferSize(Game.window,out width,out height);

			size = new Vector2Int(width,height);
			sizeFloat = (Vector2)size;
			center = new Vector2(width*0.5f,height*0.5f);
			rectangle = new RectInt(0,0,width,height);

			GLFW.GetWindowPos(Game.window,out windowX,out windowY);

			windowLocation = new Vector2Int(windowX,windowY);
			windowCenter = new Vector2(windowX+width*0.5f,windowY+height*0.5f);
		}
	}
}