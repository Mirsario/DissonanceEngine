using System;
using Dissonance.Engine.Core;
using Dissonance.Engine.Core.Attributes;
using Dissonance.Engine.Core.Modules;
using Dissonance.Framework.Graphics;
using Dissonance.Framework.Imaging;
using Dissonance.Framework.Windowing;

namespace Dissonance.Engine.Graphics
{
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	public sealed class Windowing : EngineModule
	{
		private static readonly object GlfwLock = new object();

		public IntPtr WindowHandle { get; private set; }

		protected override void PreInit()
		{
			PrepareGLFW();
			PrepareGL();

			IL.Init();
		}
		protected override void OnDispose()
		{
			if(WindowHandle!=IntPtr.Zero) {
				GLFW.SetWindowShouldClose(WindowHandle,1);
			}
		}

		private void PrepareGLFW()
		{
			Debug.Log("Preparing GLFW...");

			lock(GlfwLock) {
				GLFW.SetErrorCallback((GLFWError code,string description) => Debug.Log(code switch
				{
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

				WindowHandle = GLFW.CreateWindow(resolutionWidth,resolutionHeight,Game.DisplayName,monitor,IntPtr.Zero);

				if(WindowHandle==IntPtr.Zero) {
					throw new GraphicsException($"Unable to create a window! Make sure that your computer supports OpenGL {Rendering.OpenGLVersion}, and try updating your graphics card drivers.");
				}

				GLFW.MakeContextCurrent(WindowHandle);
				GLFW.SwapInterval(1);
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
