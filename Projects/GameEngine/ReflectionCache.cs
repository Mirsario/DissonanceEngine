using System;
using System.Linq;
using System.Reflection;

namespace GameEngine
{
	internal class ReflectionCache
	{
		public static Type[] engineTypes;
		public static Assembly engineAssembly;
		public static AssemblyName[] engineReferences;
		public static Type[] allTypes;
		public static Assembly[] allAssemblies;

		public static void Init()
		{
			engineAssembly = Assembly.GetExecutingAssembly();
			engineReferences = engineAssembly.GetReferencedAssemblies();
			engineTypes = engineAssembly.GetTypes();

			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !InternalUtils.IsMicrosoftAssembly(a) && !engineReferences.Any(r => r.Name.Equals(a.GetName().Name))).ToList();
			assemblies.Remove(engineAssembly);
			assemblies.Insert(0,engineAssembly);
			allAssemblies = assemblies.ToArray();

			allTypes = allAssemblies.SelectMany(a => a.GetTypes()).ToArray();
		}
	}
}
