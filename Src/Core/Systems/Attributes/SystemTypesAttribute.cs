using System;

namespace Dissonance.Engine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public abstract class SystemTypesAttribute : Attribute
	{
		public Type[] Types { get; protected set; }

		internal SystemTypesAttribute() { }
	}
}
