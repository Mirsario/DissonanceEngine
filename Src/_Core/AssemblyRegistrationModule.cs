using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine
{
	public sealed class AssemblyRegistrationModule : EngineModule
	{
		public delegate void AssemblyRegistrationCallback(Assembly assembly, Type[] types);

		public static event AssemblyRegistrationCallback OnAssemblyRegistered;

		private static readonly List<WeakReference<Assembly>> Assemblies = new();

		private static bool ready;

		protected override void Init()
		{
			RegisterAssembly(GetType().Assembly);
			RegisterAssembly(Game.GetType().Assembly);

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

		public static void RegisterAssembly(Assembly assembly)
		{
			if (ready) {
				OnAssemblyRegistered?.Invoke(assembly, assembly.GetTypes());
			} else {
				Assemblies.Add(new WeakReference<Assembly>(assembly));
			}
		}
	}
}
