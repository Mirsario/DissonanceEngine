using System;
using System.ComponentModel;

namespace Dissonance.Engine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
[EditorBrowsable(EditorBrowsableState.Never)] // Hide in autocompletion when possible.
public abstract class ModuleDependencyAttribute : Attribute
{
	public readonly DependencyInfo Info;

	internal ModuleDependencyAttribute(Type type, bool isOptional)
	{
		Info = new DependencyInfo(type, isOptional);
	}
}

public class ModuleDependencyAttribute<T> : ModuleDependencyAttribute
	where T : EngineModule
{
	public ModuleDependencyAttribute(bool isOptional = false) : base(typeof(T), isOptional) { }
}
