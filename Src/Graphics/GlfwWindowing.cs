using Dissonance.Framework.Graphics;
using Dissonance.Framework.Windowing;
using Dissonance.Framework.Windowing.Input;
using System;

namespace Dissonance.Engine.Graphics
{
	[ModuleAutoload(DisablingGameFlags = GameFlags.NoWindow)]
	public class GlfwWindowing : Windowing
	{
		private static readonly Vector2Int MinWindowSize = new Vector2Int(320, 240);

		private static readonly object GlfwLock = new();

		private Vector2Int windowSize;
		private Vector2Int windowLocation;
		private Vector2Int framebufferSize;
		private CursorState cursorState;

		public IntPtr WindowHandle { get; private set; }

		public override Vector2Int WindowSize => windowSize;
		public override Vector2Int WindowLocation => windowLocation;
		public override Vector2Int FramebufferSize => framebufferSize;

		public override bool ShouldClose {
			get => GLFW.WindowShouldClose(WindowHandle) == 1;
			set => GLFW.SetWindowShouldClose(WindowHandle, value ? 1 : 0);
		}
		public override CursorState CursorState {
			get => cursorState;
			set {
				GLFW.SetInputMode(WindowHandle, InputMode.Cursor, (int)value);

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

		protected override void PreInit()
		{
			lock (GlfwLock) {
				GLFW.SetErrorCallback((GLFWError code, string description) => Debug.Log(code switch {
					GLFWError.VersionUnavailable => throw new GraphicsException(description),
					_ => $"GLFW Error {code}: {description}"
				}));

				if (GLFW.Init() == 0) {
					throw new Exception("Unable to initialize GLFW!");
				}

				GLFW.WindowHint(WindowHint.ContextVersionMajor, Rendering.OpenGLVersion.Major); // Targeted major version
				GLFW.WindowHint(WindowHint.ContextVersionMinor, Rendering.OpenGLVersion.Minor); // Targeted minor version
				GLFW.WindowHint(WindowHint.OpenGLForwardCompat, 1);
				GLFW.WindowHint(WindowHint.OpenGLProfile, GLFW.OPENGL_CORE_PROFILE);

				IntPtr monitor = IntPtr.Zero;
				int resolutionWidth = 800;
				int resolutionHeight = 600;

				WindowHandle = GLFW.CreateWindow(resolutionWidth, resolutionHeight, Game.DisplayName, monitor, IntPtr.Zero);

				if (WindowHandle == IntPtr.Zero) {
					throw new GraphicsException($"Unable to create a window! Make sure that your computer supports OpenGL {Rendering.OpenGLVersion}, and try updating your graphics card drivers.");
				}

				GLFW.SetWindowSizeLimits(WindowHandle, MinWindowSize.X, MinWindowSize.Y, -1, -1);
				GLFW.MakeContextCurrent(WindowHandle);
				GLFW.SwapInterval(1);

				InitCallbacks();
				UpdateValues();
			}
		}

		protected override void OnDispose()
		{
			if (WindowHandle != IntPtr.Zero) {
				GLFW.DestroyWindow(WindowHandle);
				GLFW.Terminate();
			}

			OnKeyCallback = null;
		}

		protected override void PreRenderUpdate()
			=> UpdateValues();

		private void UpdateValues()
		{
			// Don't change resolution when minimized
			if (GLFW.GetWindowAttrib(WindowHandle, WindowAttribute.Iconified) == 0) {
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

		private static void InternalCursorPositionCallback(IntPtr windowHandle, double x, double y)
			=> Game.Instance.GetModule<GlfwWindowing>().OnCursorPositionCallback?.Invoke(x, y);

		private static void InternalMouseButtonCallback(IntPtr windowHandle, MouseButton button, MouseAction action, KeyModifiers mods)
			=> Game.Instance.GetModule<GlfwWindowing>().OnMouseButtonCallback?.Invoke(button, action, mods);

		private static void InternalScrollCallback(IntPtr windowHandle, double xOffset, double yOffset)
			=> Game.Instance.GetModule<GlfwWindowing>().OnScrollCallback?.Invoke(xOffset, yOffset);

		private static void InternalKeyCallback(IntPtr windowHandle, Keys key, int scanCode, KeyAction action, KeyModifiers mods)
			=> Game.Instance.GetModule<GlfwWindowing>().OnKeyCallback?.Invoke(key, scanCode, action, mods);

		private static void InternalCharCallback(IntPtr windowHandle, uint codePoint)
			=> Game.Instance.GetModule<GlfwWindowing>().OnCharCallback?.Invoke(codePoint);
	}
}
