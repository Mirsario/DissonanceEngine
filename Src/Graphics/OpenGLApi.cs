using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Dissonance.Engine.Graphics
{
	public static class OpenGLApi
	{
		public static GL OpenGL { get; private set; }

		internal unsafe static void GLInit()
		{
			Debug.Log("Preparing OpenGL...");

			OpenGL = GL.GetApi(Glfw.Api.Context);

			Debug.Log($"Initialized OpenGL {Marshal.PtrToStringAnsi((IntPtr)OpenGL.GetString(StringName.Version))}");
		}
	}
}
