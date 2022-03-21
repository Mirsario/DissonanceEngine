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

			var context = new GlfwContext(glfw, Game.Instance.GetModule<GlfwWindowing>().WindowHandle);

			OpenGL = GL.GetApi(context);

			Debug.Log($"Initialized OpenGL {Marshal.PtrToStringAnsi((IntPtr)OpenGL.GetString(StringName.Version))}");
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
