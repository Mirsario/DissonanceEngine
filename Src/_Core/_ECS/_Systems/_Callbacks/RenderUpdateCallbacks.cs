namespace Dissonance.Engine
{
	[ExecuteBefore<RenderUpdateCallback>]
	public sealed class BeginRenderUpdateCallback : CallbackSystem
	{

	}

	public sealed class RenderUpdateCallback : CallbackSystem
	{

	}

	[ExecuteAfter<RenderUpdateCallback>]
	public sealed class EndRenderUpdateCallback : CallbackSystem
	{

	}
}
