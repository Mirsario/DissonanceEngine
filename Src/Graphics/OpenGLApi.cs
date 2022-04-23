using System;
using System.Runtime.InteropServices;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;

namespace Dissonance.Engine.Graphics
{
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	[ModuleDependency<GlfwApi>]
	[ModuleDependency<GlfwWindowing>]
	public sealed class OpenGLApi : EngineModule
	{
		public static GL OpenGL { get; private set; }

		protected unsafe override void Init()
		{
			Debug.Log("Preparing OpenGL...");

			var glfw = GlfwApi.GLFW;

			if (glfw == null) {
				throw new InvalidOperationException("Cannot get a GLFW Api instance to initialize OpenGL.");
			}

			if (!ModuleManagement.TryGetModule(out GlfwWindowing glfwWindowing)) {
				throw new InvalidOperationException("Cannot get a GLFW Windowing instance to initialize OpenGL.");
			}

			var context = new GlfwContext(glfw, glfwWindowing.WindowHandle);

			OpenGL = GL.GetApi(context);

			var glVersion = Rendering.GetOpenGLVersion();

			Debug.Log($"Initialized OpenGL {glVersion}");

			if (glVersion < glfwWindowing.OpenGLVersion) {
				throw new GraphicsException($"Please update your graphics drivers.\r\nMinimum OpenGL version required to run this application is: {glfwWindowing.OpenGLVersion}\r\nYour OpenGL version is: {glVersion}");
			}
		}

		protected override void OnDispose()
		{
			if (OpenGL != null) {
				OpenGL.Dispose();

				OpenGL = null;
			}
		}
	}
}
