using System;

namespace Dissonance.Engine;

[AttributeUsage(AttributeTargets.Delegate | AttributeTargets.GenericParameter)]
public sealed class CallbackDeclarationAttribute : Attribute
{

}
