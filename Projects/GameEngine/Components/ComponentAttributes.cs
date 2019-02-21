using System;

namespace GameEngine
{
	[AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = true)]
	public abstract class ComponentAttribute : Attribute
	{
		public virtual void SetParameters(Type type) { }
	}
	public class AllowOnlyOnePerObject : ComponentAttribute
	{
		public override void SetParameters(Type type) => Component.typeParameters[type].allowOnlyOnePerObject = true;
	}
	public class AllowOnlyOneInWorld : ComponentAttribute
	{
		public override void SetParameters(Type type) => Component.typeParameters[type].allowOnlyOneInWorld = true;
	}
	internal class ComponentParameters
	{
		public bool allowOnlyOnePerObject = false;
		public bool allowOnlyOneInWorld = false;
	}
}