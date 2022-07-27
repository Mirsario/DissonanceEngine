using System;

namespace Dissonance.Engine;

/// <summary> Signifies that the argument for this parameter should be a component that comes from an entity available in the system's current context. </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public sealed class FromEntityAttribute : Attribute
{

}

/// <summary> Signifies that the argument for this parameter should be a component of the system's world. </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public sealed class FromWorldAttribute : Attribute
{

}

/// <summary> Signifies that the argument for this parameter should be a global component. </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public sealed class FromGlobalAttribute : Attribute
{

}
