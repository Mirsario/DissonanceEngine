using System;

namespace Dissonance.Engine.Graphics
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public abstract class AutomaticUniformDependencyAttribute : Attribute
	{
		public abstract Type Type { get; }

		internal AutomaticUniformDependencyAttribute() { }
	}

	public class AutomaticUniformDependencyAttribute<T> : AutomaticUniformDependencyAttribute
		where T : AutomaticUniform
	{
		public override Type Type => typeof(T);
	}
}
