namespace Dissonance.Engine
{
	internal sealed class RootFixedUpdateCallback : CallbackSystem
	{

	}

	[Callback<RootFixedUpdateCallback>]
	[ExecuteBefore<FixedUpdateCallback>]
	public sealed class BeginFixedUpdateCallback : CallbackSystem
	{

	}

	[Callback<RootFixedUpdateCallback>]
	public sealed class FixedUpdateCallback : CallbackSystem
	{

	}

	[Callback<RootFixedUpdateCallback>]
	[ExecuteAfter<FixedUpdateCallback>]
	public sealed class EndFixedUpdateCallback : CallbackSystem
	{

	}
}
