using Dissonance.Engine.Core;
using Dissonance.Engine.Core.Attributes;
using Dissonance.Engine.Core.Modules;
using Dissonance.Engine.Structures;
using Dissonance.Framework.Windowing;

namespace Dissonance.Engine.Graphics
{
	[ModuleDependency(true, typeof(Windowing))]
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	public sealed class Screen : EngineModule
	{
		public static bool lockCursor;

		private static CursorState cursorState;

		//Framebuffer
		public static int Width { get; private set; }
		public static int Height { get; private set; }
		public static Vector2Int Size { get; private set; }
		public static RectInt Rectangle { get; private set; }
		public static Vector2 Center { get; private set; }
		//Window
		public static int WindowX { get; internal set; }
		public static int WindowY { get; internal set; }
		public static int WindowWidth { get; internal set; }
		public static int WindowHeight { get; internal set; }
		public static Vector2Int WindowSize { get; internal set; }
		public static Vector2Int WindowLocation { get; internal set; }
		public static Vector2 WindowCenter { get; internal set; }

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

		public static CursorState CursorState {
			get => cursorState;
			set => cursorState = Game.Instance.GetModule<Windowing>().CursorState = value;
		}

		private Windowing windowing;

		protected override void Init() => UpdateValues();
		protected override void PreRenderUpdate() => UpdateValues();

		protected override void PreInit()
		{
			windowing = Game.GetModule<Windowing>(false);

			UpdateValues();
		}
		protected override void OnDispose()
		{
			windowing = null;
		}

		private void UpdateValues()
		{
			if(windowing == null) {
				return;
			}

			//Framebuffer
			Width = windowing.FramebufferSize.x;
			Height = windowing.FramebufferSize.y;

			Size = windowing.FramebufferSize;
			Center = Size * 0.5f;
			Rectangle = new RectInt(0, 0, Width, Height);

			//Window
			WindowX = windowing.WindowLocation.x;
			WindowY = windowing.WindowLocation.y;
			WindowWidth = windowing.WindowSize.x;
			WindowHeight = windowing.WindowSize.y;

			WindowSize = windowing.WindowSize;
			WindowLocation = windowing.WindowLocation;

			WindowCenter = new Vector2(WindowX + WindowWidth * 0.5f, WindowY + WindowHeight * 0.5f);
		}
	}
}
