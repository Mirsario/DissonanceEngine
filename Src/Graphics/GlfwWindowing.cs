using System;
using Silk.NET.Core.Contexts;
using Silk.NET.GLFW;
using static Dissonance.Engine.Graphics.GlfwApi;

namespace Dissonance.Engine.Graphics
{
	[Autoload(DisablingGameFlags = GameFlags.NoWindow)]
	[ModuleDependency<GlfwApi>]
	public unsafe class GlfwWindowing : Windowing
	{
		private static readonly Vector2Int MinWindowSize = new Vector2Int(320, 240);
		private static readonly Version DefaultOpenGLVersion = new(3, 2);

		private Vector2Int windowSize;
		private Vector2Int windowLocation;
		private Vector2Int framebufferSize;
		private CursorState cursorState;
		private Version openGLVersion = DefaultOpenGLVersion;
		private bool isInitialized;
		private GlfwContext glfwContext;

		public WindowHandle* WindowHandle { get; private set; }

		public Version OpenGLVersion {
			get => openGLVersion;
			set {
				if (isInitialized) {
					throw new InvalidOperationException($"OpenGL version cannot be set after the windowing module has been initialized.");
				}

				openGLVersion = value;
			}
		}

		public override GlfwContext GLContext => glfwContext;
		public override Vector2Int WindowSize => windowSize;
		public override Vector2Int WindowLocation => windowLocation;
		public override Vector2Int FramebufferSize => framebufferSize;

		public override bool ShouldClose {
			get => GLFW.WindowShouldClose(WindowHandle);
			set => GLFW.SetWindowShouldClose(WindowHandle, value);
		}
		public override CursorState CursorState {
			get => cursorState;
			set {
				// Maybe make switching extensions?
				var glfwCursorState = value switch {
					CursorState.Normal => CursorModeValue.CursorNormal,
					CursorState.Hidden => CursorModeValue.CursorHidden,
					CursorState.Disabled => CursorModeValue.CursorDisabled,
					_ => throw new IndexOutOfRangeException(),
				};

				GLFW.SetInputMode(WindowHandle, CursorStateAttribute.Cursor, glfwCursorState);

				cursorState = value;
			}
		}

		public override event CursorPositionCallback OnCursorPositionCallback;
		public override event MouseButtonCallback OnMouseButtonCallback;
		public override event ScrollCallback OnScrollCallback;
		public override event KeyCallback OnKeyCallback;
		public override event CharCallback OnCharCallback;

		public override void SwapBuffers()
			=> GLFW.SwapBuffers(WindowHandle);

		public override bool SetVideoMode(int width, int height)
		{
			if (width <= 0 || height <= 0) {
				throw new ArgumentException($"'{nameof(width)}' and '{nameof(height)}' cannot be less than or equal to zero.");
			}

			GLFW.SetWindowSize(WindowHandle, width, height);
			UpdateValues();

			return true;
		}

		protected override void Init()
		{
			GLFW.SetErrorCallback((ErrorCode code, string description) => Debug.Log(code switch {
				ErrorCode.VersionUnavailable => throw new GraphicsException(description),
				_ => $"GLFW Error {code}: {description}"
			}));

			if (!GLFW.Init()) {
				throw new Exception("Unable to initialize GLFW!");
			}

			GLFW.WindowHint(WindowHintInt.ContextVersionMajor, OpenGLVersion.Major); // Targeted major version
			GLFW.WindowHint(WindowHintInt.ContextVersionMinor, OpenGLVersion.Minor); // Targeted minor version
			GLFW.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
			GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

			int resolutionWidth = 800;
			int resolutionHeight = 600;

			WindowHandle = GLFW.CreateWindow(resolutionWidth, resolutionHeight, Game.Instance.DisplayName, null, null);

			if (WindowHandle == null) {
				throw new GraphicsException($"Unable to create a window! Make sure that your computer supports OpenGL {OpenGLVersion}, and try updating your graphics card drivers.");
			}

			glfwContext = new GlfwContext(GLFW, WindowHandle);

			GLFW.SetWindowSizeLimits(WindowHandle, MinWindowSize.X, MinWindowSize.Y, -1, -1);
			GLFW.MakeContextCurrent(WindowHandle);
			GLFW.SwapInterval(1);

			InitCallbacks();
			UpdateValues();
			
			isInitialized = true;
		}

		protected override void OnDispose()
		{
			if (WindowHandle != null) {
				GLFW.DestroyWindow(WindowHandle);
				GLFW.Terminate();
			}

			OnKeyCallback = null;
		}

		protected override void PreRenderUpdate()
			=> Update();
		
		protected override void PreFixedUpdate()
			=> Update();

		private void Update()
		{
			GLFW.PollEvents();
			UpdateValues();
		}

		private void UpdateValues()
		{
			// Don't change resolution when minimized
			if (!GLFW.GetWindowAttrib(WindowHandle, WindowAttributeGetter.Iconified)) {
				// Framebuffer
				GLFW.GetFramebufferSize(WindowHandle, out framebufferSize.X, out framebufferSize.Y);

				// Window
				GLFW.GetWindowSize(WindowHandle, out windowSize.X, out windowSize.Y);
				GLFW.GetWindowPos(WindowHandle, out windowLocation.X, out windowLocation.Y);
			}

			framebufferSize.X = Math.Max(1, framebufferSize.X);
			framebufferSize.Y = Math.Max(1, framebufferSize.Y);
			windowSize.X = Math.Max(1, windowSize.X);
			windowSize.Y = Math.Max(1, windowSize.Y);
		}

		private void InitCallbacks()
		{
			GLFW.SetCursorPosCallback(WindowHandle, InternalCursorPositionCallback);
			GLFW.SetMouseButtonCallback(WindowHandle, InternalMouseButtonCallback);
			GLFW.SetScrollCallback(WindowHandle, InternalScrollCallback);
			GLFW.SetKeyCallback(WindowHandle, InternalKeyCallback);
			GLFW.SetCharCallback(WindowHandle, InternalCharCallback);
		}

		private static void InternalCursorPositionCallback(WindowHandle* windowHandle, double x, double y)
			=> ModuleManagement.GetModule<GlfwWindowing>().OnCursorPositionCallback?.Invoke(x, y);

		private static void InternalMouseButtonCallback(WindowHandle* windowHandle, MouseButton button, InputAction action, KeyModifiers mods)
			=> ModuleManagement.GetModule<GlfwWindowing>().OnMouseButtonCallback?.Invoke(button, action, mods);

		private static void InternalScrollCallback(WindowHandle* windowHandle, double xOffset, double yOffset)
			=> ModuleManagement.GetModule<GlfwWindowing>().OnScrollCallback?.Invoke(xOffset, yOffset);

		private static void InternalKeyCallback(WindowHandle* windowHandle, Keys key, int scanCode, InputAction action, KeyModifiers mods)
			=> ModuleManagement.GetModule<GlfwWindowing>().OnKeyCallback?.Invoke(key, scanCode, action, mods);

		private static void InternalCharCallback(WindowHandle* windowHandle, uint codePoint)
			=> ModuleManagement.GetModule<GlfwWindowing>().OnCharCallback?.Invoke(codePoint);
	}
}
