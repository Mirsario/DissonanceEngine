using System;

namespace Dissonance.Engine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public abstract class SystemTypeDataAttribute : Attribute
{
	public abstract void ModifySystemTypeData(SystemTypeData systemTypeData);
}
