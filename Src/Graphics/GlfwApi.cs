using Silk.NET.GLFW;

namespace Dissonance.Engine.Graphics
{
	public static class GlfwApi
	{
		public static Glfw GLFW { get; private set; }

		internal static void InitGlfw()
		{
			GLFW = Glfw.GetApi();
		}

		internal static void CleanupGlfw()
		{
			if (GLFW != null) {
				GLFW.Dispose();

				GLFW = null;
			}
		}
	}
}
