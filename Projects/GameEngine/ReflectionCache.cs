using System;
using System.Reflection;

namespace GameEngine
{
	internal class ReflectionCache
	{
		public static Assembly engineAssembly;
		public static Type[] engineTypes;

		public static void Init()
		{
			engineAssembly = Assembly.GetExecutingAssembly();
			engineTypes = engineAssembly.GetTypes();
			//vv Don't remember what this was about.
			//var references = engineAssembly.GetReferencedAssemblies();
			//var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !Utils.IsMicrosoftAssembly(a) && !references.Any(r => r.Name.Equals(a.GetName().Name))).ToList();
			//assemblies.Remove(engineAssembly);
			//assemblies.Insert(0,engineAssembly);
		}
	}
}
