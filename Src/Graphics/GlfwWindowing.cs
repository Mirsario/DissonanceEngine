using System;
using Silk.NET.GLFW;

namespace Dissonance.Engine.Graphics
{
	[ModuleAutoload(DisablingGameFlags = GameFlags.NoWindow)]
	public unsafe class GlfwWindowing : Windowing
	{
		private static readonly Vector2Int MinWindowSize = new Vector2Int(320, 240);

		private static readonly object GlfwLock = new();

		private Vector2Int windowSize;
		private Vector2Int windowLocation;
		private Vector2Int framebufferSize;
		private CursorModeValue cursorState;

		public WindowHandle* WindowHandle { get; private set; }

		public override Vector2Int WindowSize => windowSize;
		public override Vector2Int WindowLocation => windowLocation;
		public override Vector2Int FramebufferSize => framebufferSize;

		public override bool ShouldClose {
			get => Glfw.Api.WindowShouldClose(WindowHandle);
			set => Glfw.Api.SetWindowShouldClose(WindowHandle, value);
		}
		public override CursorModeValue CursorState {
			get => cursorState;
			set {
				Glfw.Api.SetInputMode(WindowHandle, CursorStateAttribute.Cursor, value);

				cursorState = value;
			}
		}

		public override event CursorPositionCallback OnCursorPositionCallback;
		public override event MouseButtonCallback OnMouseButtonCallback;
		public override event ScrollCallback OnScrollCallback;
		public override event KeyCallback OnKeyCallback;
		public override event CharCallback OnCharCallback;

		public override void SwapBuffers()
			=> Glfw.Api.SwapBuffers(WindowHandle);

		public override bool SetVideoMode(int width, int height)
		{
			if (width <= 0 || height <= 0) {
				throw new ArgumentException($"'{nameof(width)}' and '{nameof(height)}' cannot be less than or equal to zero.");
			}

			Glfw.Api.SetWindowSize(WindowHandle, width, height);
			UpdateValues();

			return true;
		}

		protected override void PreInit()
		{
			lock (GlfwLock) {
				Glfw.Api.SetErrorCallback((ErrorCode code, string description) => Debug.Log(code switch {
					ErrorCode.VersionUnavailable => throw new GraphicsException(description),
					_ => $"GLFW Error {code}: {description}"
				}));

				if (!Glfw.Api.Init()) {
					throw new Exception("Unable to initialize GLFW!");
				}

				Glfw.Api.WindowHint(WindowHintInt.ContextVersionMajor, Rendering.OpenGLVersion.Major); // Targeted major version
				Glfw.Api.WindowHint(WindowHintInt.ContextVersionMinor, Rendering.OpenGLVersion.Minor); // Targeted minor version
				Glfw.Api.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
				Glfw.Api.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

				int resolutionWidth = 800;
				int resolutionHeight = 600;

				WindowHandle = Glfw.Api.CreateWindow(resolutionWidth, resolutionHeight, Game.DisplayName, null, null);

				if (WindowHandle == default) {
					throw new GraphicsException($"Unable to create a window! Make sure that your computer supports OpenGL {Rendering.OpenGLVersion}, and try updating your graphics card drivers.");
				}

				Glfw.Api.SetWindowSizeLimits(WindowHandle, MinWindowSize.X, MinWindowSize.Y, -1, -1);
				Glfw.Api.MakeContextCurrent(WindowHandle);
				Glfw.Api.SwapInterval(1);

				InitCallbacks();
				UpdateValues();
			}
		}

		protected override void OnDispose()
		{
			if (WindowHandle != null) {
				Glfw.Api.DestroyWindow(WindowHandle);
				Glfw.Api.Terminate();
			}

			OnKeyCallback = null;
		}

		protected override void PreRenderUpdate()
			=> UpdateValues();

		private void UpdateValues()
		{
			// Don't change resolution when minimized
			if (!Glfw.Api.GetWindowAttrib(WindowHandle, WindowAttributeGetter.Iconified)) {
				// Framebuffer
				Glfw.Api.GetFramebufferSize(WindowHandle, out framebufferSize.X, out framebufferSize.Y);

				// Window
				Glfw.Api.GetWindowSize(WindowHandle, out windowSize.X, out windowSize.Y);
				Glfw.Api.GetWindowPos(WindowHandle, out windowLocation.X, out windowLocation.Y);
			}

			framebufferSize.X = Math.Max(1, framebufferSize.X);
			framebufferSize.Y = Math.Max(1, framebufferSize.Y);
			windowSize.X = Math.Max(1, windowSize.X);
			windowSize.Y = Math.Max(1, windowSize.Y);
		}

		private void InitCallbacks()
		{
			Glfw.Api.SetCursorPosCallback(WindowHandle, InternalCursorPositionCallback);
			Glfw.Api.SetMouseButtonCallback(WindowHandle, InternalMouseButtonCallback);
			Glfw.Api.SetScrollCallback(WindowHandle, InternalScrollCallback);
			Glfw.Api.SetKeyCallback(WindowHandle, InternalKeyCallback);
			Glfw.Api.SetCharCallback(WindowHandle, InternalCharCallback);
		}

		private static void InternalCursorPositionCallback(WindowHandle* windowHandle, double x, double y)
			=> Game.Instance.GetModule<GlfwWindowing>().OnCursorPositionCallback?.Invoke(x, y);

		private static void InternalMouseButtonCallback(WindowHandle* windowHandle, MouseButton button, InputAction action, KeyModifiers mods)
			=> Game.Instance.GetModule<GlfwWindowing>().OnMouseButtonCallback?.Invoke(button, action, mods);

		private static void InternalScrollCallback(WindowHandle* windowHandle, double xOffset, double yOffset)
			=> Game.Instance.GetModule<GlfwWindowing>().OnScrollCallback?.Invoke(xOffset, yOffset);

		private static void InternalKeyCallback(WindowHandle* windowHandle, Keys key, int scanCode, InputAction action, KeyModifiers mods)
			=> Game.Instance.GetModule<GlfwWindowing>().OnKeyCallback?.Invoke(key, scanCode, action, mods);

		private static void InternalCharCallback(WindowHandle* windowHandle, uint codePoint)
			=> Game.Instance.GetModule<GlfwWindowing>().OnCharCallback?.Invoke(codePoint);
	}
}
