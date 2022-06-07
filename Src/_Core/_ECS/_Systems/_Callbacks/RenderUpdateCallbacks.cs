namespace Dissonance.Engine;

public sealed class RootRenderUpdateCallback : CallbackSystem
{

}

[Callback<RootRenderUpdateCallback>]
[ExecuteBefore<RenderUpdateCallback>]
public sealed class BeginRenderUpdateCallback : CallbackSystem
{

}

[Callback<RootRenderUpdateCallback>]
public sealed class RenderUpdateCallback : CallbackSystem
{

}

[Callback<RootRenderUpdateCallback>]
[ExecuteAfter<RenderUpdateCallback>]
public sealed class EndRenderUpdateCallback : CallbackSystem
{

}
