using System;
using System.Linq;

namespace Dissonance.Engine.Core.Modules
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class ModuleDependencyAttribute : Attribute
	{
		public readonly ModuleDependency[] Dependencies;

		public ModuleDependencyAttribute(params Type[] dependencies) : this(false, dependencies) { }
		public ModuleDependencyAttribute(bool optional, params Type[] dependencies)
		{
			foreach(var type in dependencies) {
				if(type.IsAbstract) {
					throw new ArgumentException($"Dependency type '{type.Name}' is invalid, as it's abstract.");
				}

				if(!typeof(EngineModule).IsAssignableFrom(type)) {
					throw new ArgumentException($"Dependency type '{type.Name}' is invalid, as it does not derive from '{nameof(EngineModule)}'.");
				}
			}

			Dependencies = dependencies
				.Select(type => new ModuleDependency(type, optional))
				.ToArray();
		}
	}
}
