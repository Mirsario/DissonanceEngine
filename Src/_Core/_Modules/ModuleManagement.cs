using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.Utilities;

#nullable enable

namespace Dissonance.Engine;

public static unsafe class ModuleManagement
{
	internal static readonly EngineModuleHooks Hooks = new();
	
	private static readonly List<EngineModule> modules = new();
	private static readonly Dictionary<Type, List<EngineModule>> modulesByExactOrDerivingType = new();
	
	private static bool modulesPreInitialized;
	private static bool initialized;
	private static bool preInitialized;

	public static void PreInitialize()
	{
		if (preInitialized) {
			return;
		}

		AssemblyManagement.AddAssemblyCallback(OnAssemblyRegistered);

		preInitialized = true;
	}

	public static void Initialize()
	{
		if (initialized) {
			return;
		}

		PreInitialize();

		RefreshModules();

		foreach (var module in modules) {
			module.InvokePreInitialize();
		}

		modulesPreInitialized = true;

		foreach (var assembly in AssemblyManagement.EnumerateAssemblies()) {
			foreach (var module in modules) {
				module.InvokeInitializeForAssembly(assembly);
			}
		}

		foreach (var module in modules) {
			module.InvokeInitialize();
		}

		initialized = true;
	}

	public static void Unload()
	{
		if (modules != null) {
			for (int i = 0; i < modules.Count; i++) {
				modules[i]?.Dispose();
			}

			modules.Clear();
		}
	}

	public static void AddModule(EngineModule module)
	{
		modules.Add(module);

		foreach (var type in ReflectionUtils.EnumerateBaseTypes(module.GetType(), includingOriginal: true, stopAt: typeof(EngineModule))) {
			if (!modulesByExactOrDerivingType.TryGetValue(type, out var list)) {
				modulesByExactOrDerivingType[type] = list = new List<EngineModule>();
			}

			list.Add(module);
		}

		if (modulesPreInitialized) {
			RefreshModules();

			module.InvokePreInitialize();
		}

		if (initialized) {
			module.InvokeInitialize();
		}
	}

	public static bool RemoveModule(EngineModule module)
	{
		if (!modules.Remove(module)) {
			return false;
		}

		foreach (var pair in modulesByExactOrDerivingType) {
			pair.Value.Remove(module);
		}

		return true;
	}

	public static bool TryGetModule<T>([NotNullWhen(returnValue: true)] out T? result) where T : EngineModule
	{
		for (int i = 0; i < modules.Count; i++) {
			if (modules[i] is T t) {
				result = t;

				return true;
			}
		}

		result = null;

		return false;
	}

	public static bool TryGetModule(Type type, [NotNullWhen(returnValue: true)] out EngineModule? result)
	{
		if (modulesByExactOrDerivingType == null || !modulesByExactOrDerivingType.TryGetValue(type, out var list) || list.Count == 0) {
			result = null;

			return false;
		}

		result = list[0];

		return true;
	}

	public static T GetModule<T>() where T : EngineModule
	{
		if (TryGetModule<T>(out var result)) {
			return result;
		}

		throw new ArgumentException($"The current {nameof(Game)} instance does not contain a '{typeof(T).Name}' engine module.");
	}

	public static EngineModule GetModule(Type type)
	{
		if (TryGetModule(type, out var result)) {
			return result;
		}

		throw new ArgumentException($"The current {nameof(Game)} instance does not contain a '{type.Name}' engine module.");
	}

	internal static List<EngineModule> AutoloadModules(Assembly assembly)
	{
		var newModules = new List<EngineModule>();

		foreach (var type in assembly.GetTypes()) {
			if (type.IsAbstract || !typeof(EngineModule).IsAssignableFrom(type)) {
				continue;
			}

			if (!AutoloadAttribute.TypeNeedsAutoloading(type)) {
				continue;
			}

			var module = (EngineModule)Activator.CreateInstance(type, nonPublic: true)!;

			AddModule(module);

			newModules.Add(module);
		}

		if (initialized) {
			RefreshModules();
		}

		return newModules;
	}

	internal static void RefreshModules()
	{
		SortModules();
		RebuildModuleHooks();
	}

	private static void SortModules()
	{
		static IEnumerable<int> GetDirectDependencyIndices(EngineModule module)
		{
			if (module.Dependencies == null) {
				yield break;
			}

			foreach (var dependency in module.Dependencies) {
				if (!TryGetModule(dependency.Type, out var result)) {
					if (!dependency.Optional) {
						throw new Exception($"Unable to find module of type '{dependency.Type.Name}', required by module '{module.GetType().Name}'.");
					}

					continue;
				}
				
				yield return result.DependencyIndex;
			}
		}

		for (int i = 0; i < modules.Count; i++) {
			modules[i].DependencyIndex = i;
		}

		modules.DependencySort(GetDirectDependencyIndices, true);

		for (int i = 0; i < modules.Count; i++) {
			modules[i].DependencyIndex = i;
		}
	}

	private static void RebuildModuleHooks()
	{
		static int CustomSorting((EngineModule module, Delegate method, int position) tupleA, (EngineModule module, Delegate method, int position) tupleB)
			=> tupleA.module.DependencyIndex < tupleB.module.DependencyIndex && tupleA.position < tupleB.position ? -1 : 1;

		HookUtils.BuildHooksFromVirtualMethods(modules, Hooks, customSorting: CustomSorting);
	}

	private static void OnAssemblyRegistered(Assembly assembly, Type[] types)
	{
		// Load assembly modules

		AutoloadModules(assembly);

		if (modulesPreInitialized) {
			foreach (var module in modules) {
				module.InvokeInitializeForAssembly(assembly);
			}
		}
	}
}
