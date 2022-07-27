namespace Dissonance.Engine;

[ExecuteBefore<FixedUpdateCallback>]
public sealed class BeginFixedUpdateCallback : CallbackSystem
{

}

public sealed class FixedUpdateCallback : CallbackSystem
{

}

[ExecuteAfter<FixedUpdateCallback>]
public sealed class EndFixedUpdateCallback : CallbackSystem
{

}
