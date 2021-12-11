namespace Dissonance.Engine
{
	internal sealed class RootFixedUpdateCallback : CallbackSystem
	{

	}

	[Callback<RootFixedUpdateCallback>]
	public sealed class EarlyFixedUpdateCallback : CallbackSystem
	{

	}

	[Callback<RootFixedUpdateCallback>]
	[ExecuteAfter<EarlyFixedUpdateCallback>]
	[ExecuteBefore<LateFixedUpdateCallback>]
	public sealed class FixedUpdateCallback : CallbackSystem
	{

	}

	[Callback<RootFixedUpdateCallback>]
	public sealed class LateFixedUpdateCallback : CallbackSystem
	{

	}
}
