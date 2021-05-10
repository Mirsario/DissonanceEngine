using System;
using System.Linq;

namespace Dissonance.Engine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class SystemDependencyAttribute : Attribute
	{
		public readonly DependencyInfo[] Dependencies;

		public SystemDependencyAttribute(params Type[] dependencies) : this(false, dependencies) { }
		public SystemDependencyAttribute(bool optional, params Type[] dependencies)
		{
			foreach(var type in dependencies) {
				if(!typeof(SystemBase).IsAssignableFrom(type)) {
					throw new ArgumentException($"Dependency type '{type.Name}' is invalid, as it does not derive from '{nameof(SystemBase)}'.");
				}
			}

			Dependencies = dependencies
				.Select(type => new DependencyInfo(type, optional))
				.ToArray();
		}
	}
}
