using System;
using System.Runtime.InteropServices;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;

namespace Dissonance.Engine.Graphics
{
	public static class OpenGLApi
	{
		public static GL OpenGL { get; private set; }

		internal unsafe static void InitOpenGL(Glfw glfw)
		{
			Debug.Log("Preparing OpenGL...");

			var context = new GlfwContext(glfw, Game.Instance.GetModule<GlfwWindowing>().WindowHandle);

			OpenGL = GL.GetApi(context);

			Debug.Log($"Initialized OpenGL {Marshal.PtrToStringAnsi((IntPtr)OpenGL.GetString(StringName.Version))}");
		}

		internal static void CleanupOpenGL()
		{
			if (OpenGL != null) {
				OpenGL.Dispose();

				OpenGL = null;
			}
		}
	}
}
