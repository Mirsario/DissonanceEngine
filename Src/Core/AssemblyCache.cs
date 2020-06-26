using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine.Core
{
	public static class AssemblyCache
	{
		public static Type[] AllTypes { get; private set; }
		public static Type[] EngineTypes { get; private set; }
		public static Assembly[] AllAssemblies { get; private set; }
		public static Assembly EngineAssembly { get; private set; }
		public static AssemblyName[] EngineReferences { get; private set; }

		public static void Init()
		{
			EngineAssembly = Assembly.GetExecutingAssembly();
			EngineReferences = EngineAssembly.GetReferencedAssemblies();
			EngineTypes = EngineAssembly.GetTypes();

			var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

			//Attempt to load assemblies that are referenced, but weren't loaded yet.
			foreach(var assembly in loadedAssemblies) {
				var references = assembly.GetReferencedAssemblies();

				foreach(var reference in references) {
					if(!loadedAssemblies.Any(a => a.GetName()==reference)) {
						try {
							Assembly.Load(reference);
						}
						catch { }
					}
				}
			}

			var assemblies = AppDomain.CurrentDomain
				.GetAssemblies()
				.Where(a => !a.IsDynamic && !a.GetName().Name.StartsWith("System.") && !EngineReferences.Any(r => r.Name.Equals(a.GetName().Name))).ToList();

			assemblies.Remove(EngineAssembly);
			assemblies.Insert(0,EngineAssembly);

			AllAssemblies = assemblies.ToArray();
			AllTypes = AllAssemblies.SelectMany(a => a.GetTypes()).ToArray();
		}
	}
}
