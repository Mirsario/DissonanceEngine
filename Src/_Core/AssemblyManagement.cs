using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable

namespace Dissonance.Engine;

public static class AssemblyManagement
{
	public delegate void AssemblyRegistrationCallback(Assembly assembly, Type[] types);

	private static readonly List<WeakReference<Assembly>> assemblies = new();

	private static AssemblyRegistrationCallback? onAssemblyRegistered;
	
	static AssemblyManagement()
	{
		RegisterAssembly(typeof(AssemblyManagement).Assembly);
	}

	public static void RegisterAssembly(Assembly assembly)
	{
		lock (assemblies) {
			onAssemblyRegistered?.Invoke(assembly, assembly.GetTypes());

			assemblies.Add(new WeakReference<Assembly>(assembly));
		}
	}

	/// <summary>
	/// Registers a callback to be invoked for every future registered assembly, and also every assembly that has already been registered.
	/// </summary>
	/// <param name="callback"> The callback to execute for every assembly that will be or has been registered. </param>
	public static void AddAssemblyCallback(AssemblyRegistrationCallback callback)
	{
		onAssemblyRegistered += callback;

		foreach (var assembly in EnumerateAssemblies()) {
			onAssemblyRegistered?.Invoke(assembly, assembly.GetTypes());
		}
	}

	/// <summary>
	/// Unregisters a
	/// </summary>
	/// <param name="callback"></param>
	public static void RemoveAssemblyCallback(AssemblyRegistrationCallback callback)
	{
		onAssemblyRegistered -= callback;
	}

	public static IEnumerable<Assembly> EnumerateAssemblies()
	{
		lock (assemblies) {
			for (int i = 0; i < assemblies.Count; i++) {
				var weakRef = assemblies[i];

				if (!weakRef.TryGetTarget(out var assembly)) {
					assemblies.RemoveAt(i--);
					continue;
				}

				yield return assembly;
			}
		}
	}
}
