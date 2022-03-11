using SilkGlfw = Silk.NET.GLFW.Glfw;

namespace Dissonance.Engine.Graphics
{
	public static class Glfw
	{
		public static SilkGlfw Api { get; private set; }

		internal static void Initialize()
		{
			Api = SilkGlfw.GetApi();
		}

		internal static void Cleanup()
		{
			if (Api != null) {
				Api.Dispose();

				Api = null;
			}
		}
	}
}
