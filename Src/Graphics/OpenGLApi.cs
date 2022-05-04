using System;
using Silk.NET.OpenGL;

namespace Dissonance.Engine.Graphics
{
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	[ModuleDependency<Windowing>]
	public class OpenGLApi : EngineModule
	{
		public static GL OpenGL { get; protected set; }

		protected unsafe override void Init()
		{
			Debug.Log("Preparing OpenGL...");

			if (!ModuleManagement.TryGetModule(out Windowing windowing)) {
				throw new InvalidOperationException($"Cannot get a {nameof(Windowing)} module instance to initialize OpenGL.");
			}

			OpenGL = GL.GetApi(windowing);

			var glVersion = Rendering.GetOpenGLVersion();

			Debug.Log($"Initialized OpenGL {glVersion}");
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
