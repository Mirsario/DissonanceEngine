using Silk.NET.GLFW;

namespace Dissonance.Engine.Graphics
{
	public sealed class GlfwApi : EngineModule
	{
		public static Glfw GLFW { get; private set; }

		protected override void Init()
		{
			GLFW = Glfw.GetApi();
		}

		protected override void OnDispose()
		{
			if (GLFW != null) {
				GLFW.Dispose();

				GLFW = null;
			}
		}
	}
}
