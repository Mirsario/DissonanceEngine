using System;

namespace Dissonance.Engine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public abstract class SystemTypeDataAttribute : Attribute
	{
		internal SystemTypeDataAttribute() { }

		public abstract void ModifySystemTypeData(SystemTypeData systemTypeData);
	}
}
