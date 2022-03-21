using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine
{
	public static class AssemblyManagement
	{
		public delegate void AssemblyRegistrationCallback(Assembly assembly, Type[] types);

		public static event AssemblyRegistrationCallback OnAssemblyRegistered;

		private static readonly List<WeakReference<Assembly>> Assemblies = new();

		private static bool ready;

		public static void RegisterAssembly(Assembly assembly)
		{
			if (ready) {
				OnAssemblyRegistered?.Invoke(assembly, assembly.GetTypes());
			} else {
				Assemblies.Add(new WeakReference<Assembly>(assembly));
			}
		}

		public static IEnumerable<Assembly> EnumerateAssemblies()
		{
			foreach (var weakRef in Assemblies) {
				if (weakRef.TryGetTarget(out var assembly)) {
					yield return assembly;
				}
			}
		}

		internal static void Initialize()
		{
			RegisterAssembly(typeof(AssemblyManagement).Assembly);
			RegisterAssembly(Game.Instance.GetType().Assembly);

			ready = true;

			for (int i = 0; i < Assemblies.Count; i++) {
				var weakRef = Assemblies[i];

				if (!weakRef.TryGetTarget(out var assembly)) {
					Assemblies.RemoveAt(i--);
					continue;
				}

				OnAssemblyRegistered?.Invoke(assembly, assembly.GetTypes());
			}
		}
	}
}
