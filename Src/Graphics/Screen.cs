namespace Dissonance.Engine.Graphics
{
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	[ModuleDependency<Windowing>(isOptional: true)]
	public sealed class Screen : EngineModule
	{
		private static Windowing windowing;

		// Framebuffer
		public static int Width { get; private set; }
		public static int Height { get; private set; }
		public static Vector2Int Size { get; private set; }
		public static RectInt Rectangle { get; private set; }
		public static Vector2 Center { get; private set; }
		// Window
		public static int WindowX { get; internal set; }
		public static int WindowY { get; internal set; }
		public static int WindowWidth { get; internal set; }
		public static int WindowHeight { get; internal set; }
		public static Vector2Int WindowSize { get; internal set; }
		public static Vector2Int WindowLocation { get; internal set; }
		public static Vector2 WindowCenter { get; internal set; }

		/*public static bool Fullscreen {
			get => Glfw.Api.GetWindowMonitor(Rendering.window)!=IntPtr.Zero;
			set {
				var monitor = Glfw.Api.GetWindowMonitor(Rendering.window);
				bool isFullscreen = monitor!=IntPtr.Zero;

				if (value!=isFullscreen) {
					return;
				}

				if (value) {
					Glfw.Api.SetWindowMonitor(Rendering.window,Glfw.Api.GetPrimaryMonitor(),0,0,800,600,144);
				} else {
					var videoMode = Glfw.Api.GetVideoMode(monitor);

					Glfw.Api.SetWindowMonitor(Rendering.window,IntPtr.Zero,0,0,videoMode.width,videoMode.height,videoMode.refreshRate);
				}
			}
		}*/

		public static CursorState CursorState {
			get => Game.Instance.GetModule<Windowing>().CursorState;
			set => Game.Instance.GetModule<Windowing>().CursorState = value;
		}

		protected override void Init()
		{
			windowing = Game.GetModule<Windowing>(false);

			UpdateValues();
		}

		protected override void PreRenderUpdate()
			=> UpdateValues();

		protected override void OnDispose()
		{
			windowing = null;
		}

		internal static void UpdateValues()
		{
			if (windowing == null) {
				return;
			}

			// Framebuffer
			Width = windowing.FramebufferSize.X;
			Height = windowing.FramebufferSize.Y;

			Size = windowing.FramebufferSize;
			Center = Size * 0.5f;
			Rectangle = new RectInt(0, 0, Width, Height);

			// Window
			WindowX = windowing.WindowLocation.X;
			WindowY = windowing.WindowLocation.Y;
			WindowWidth = windowing.WindowSize.X;
			WindowHeight = windowing.WindowSize.Y;

			WindowSize = windowing.WindowSize;
			WindowLocation = windowing.WindowLocation;

			WindowCenter = new Vector2(WindowX + WindowWidth * 0.5f, WindowY + WindowHeight * 0.5f);
		}
	}
}
