using System;

namespace GameEngine
{
	[AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = true)]
	public abstract class ComponentAttribute : Attribute
	{
		public virtual void SetParameters(Type type) { }
	}
}