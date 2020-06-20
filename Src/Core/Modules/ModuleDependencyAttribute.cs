using System;

namespace Dissonance.Engine.Core.Modules
{
	[AttributeUsage(AttributeTargets.Class,AllowMultiple = true,Inherited = true)]
	public class ModuleDependencyAttribute : Attribute
	{
		public readonly Type[] Dependencies;

		public ModuleDependencyAttribute(params Type[] dependencies)
		{
			foreach(var type in dependencies) {
				if(type.IsAbstract) {
					throw new ArgumentException($"Dependency type '{type.Name}' is invalid, as it's abstract.");
				}

				if(!typeof(EngineModule).IsAssignableFrom(type)) {
					throw new ArgumentException($"Dependency type '{type.Name}' is invalid, as it does not derive from '{nameof(EngineModule)}'.");
				}
			}

			Dependencies = dependencies;
		}
	}
}
