using System;

namespace Dissonance.Engine.Core.Components.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public abstract class ComponentAttribute : Attribute
	{
		public abstract void SetParameters(Type type, ComponentParameters parameters);
	}
}