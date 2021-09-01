using System;
using System.Linq;

namespace Dissonance.Engine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class ModuleDependencyAttribute : Attribute
	{
		public readonly DependencyInfo[] Dependencies;

		public ModuleDependencyAttribute(params Type[] dependencies) : this(false, dependencies) { }

		public ModuleDependencyAttribute(bool optional, params Type[] dependencies)
		{
			foreach (var type in dependencies) {
				if (!typeof(EngineModule).IsAssignableFrom(type)) {
					throw new ArgumentException($"Dependency type '{type.Name}' is invalid, as it does not derive from '{nameof(EngineModule)}'.");
				}
			}

			Dependencies = dependencies
				.Select(type => new DependencyInfo(type, optional))
				.ToArray();
		}
	}
}
