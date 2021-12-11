namespace Dissonance.Engine
{
	internal sealed class RootRenderUpdateCallback : CallbackSystem
	{

	}

	[Callback<RootRenderUpdateCallback>]
	public sealed class EarlyRenderUpdateCallback : CallbackSystem
	{

	}

	[Callback<RootRenderUpdateCallback>]
	[ExecuteAfter<EarlyRenderUpdateCallback>]
	[ExecuteBefore<LateRenderUpdateCallback>]
	public sealed class RenderUpdateCallback : CallbackSystem
	{

	}

	[Callback<RootRenderUpdateCallback>]
	public sealed class LateRenderUpdateCallback : CallbackSystem
	{

	}
}
