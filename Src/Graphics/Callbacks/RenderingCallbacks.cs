namespace Dissonance.Engine.Graphics
{
	[Callback<RootRenderUpdateCallback>]
	[ExecuteAfter<EndRenderUpdateCallback>]
	internal sealed class RootRenderingCallback : CallbackSystem
	{
		
	}

	[Callback<RootRenderingCallback>]
	public sealed class RenderingCallback : CallbackSystem
	{

	}
}
