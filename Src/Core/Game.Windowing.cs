using System;
using Dissonance.Engine.Graphics;
using Dissonance.Framework.Graphics;
using Dissonance.Framework.Windowing;

namespace Dissonance.Engine
{
	partial class Game
	{
		private static readonly object GlfwLock = new object();

		private void PrepareGLFW()
		{
			Debug.Log("Preparing GLFW...");

			lock(GlfwLock) {
				GLFW.SetErrorCallback((GLFWError code,string description) => Debug.Log(code switch {
					GLFWError.VersionUnavailable => throw new GraphicsException(description),
					_ => $"GLFW Error {code}: {description}"
				}));

				if(GLFW.Init()==0) {
					throw new Exception("Unable to initialize GLFW!");
				}

				GLFW.WindowHint(WindowHint.ContextVersionMajor,Rendering.OpenGLVersion.Major); //Targeted major version
				GLFW.WindowHint(WindowHint.ContextVersionMinor,Rendering.OpenGLVersion.Minor); //Targeted minor version
				GLFW.WindowHint(WindowHint.OpenGLForwardCompat,1);
				GLFW.WindowHint(WindowHint.OpenGLProfile,GLFW.OPENGL_CORE_PROFILE);

				IntPtr monitor = IntPtr.Zero;
				int resolutionWidth = 800;
				int resolutionHeight = 600;

				window = GLFW.CreateWindow(resolutionWidth,resolutionHeight,displayName,monitor,IntPtr.Zero);

				if(window==IntPtr.Zero) {
					throw new GraphicsException($"Unable to create a window! Make sure that your computer supports OpenGL {Rendering.OpenGLVersion}, and try updating your graphics card drivers.");
				}

				GLFW.MakeContextCurrent(window);
				GLFW.SwapInterval(0);
			}

			Debug.Log("Initialized GLFW.");
		}
		private void PrepareGL()
		{
			Debug.Log("Preparing OpenGL...");

			GL.Load(Rendering.OpenGLVersion);

			Rendering.CheckGLErrors("Post GL.Load()");

			Debug.Log($"Initialized OpenGL {GL.GetString(StringName.Version)}");
		}
	}
}