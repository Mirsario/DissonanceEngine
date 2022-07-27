using Silk.NET.GLFW;

namespace Dissonance.Engine.Graphics;

[Autoload(DisablingGameFlags = GameFlags.NoWindow)]
public class GlfwApi : EngineModule
{
	public static Glfw GLFW { get; protected set; }

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
