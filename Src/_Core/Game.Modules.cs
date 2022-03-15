using System;
using System.Collections.Generic;
using System.Linq;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	partial class Game
	{
		private bool modulesReady;
		private List<EngineModule> modules = new();
		private Dictionary<Type, List<EngineModule>> modulesByType = new();
		private EngineModuleHooks moduleHooks = new();

		public void AddModule(EngineModule module)
		{
			modules.Add(module);

			foreach (var type in ReflectionUtils.EnumerateBaseTypes(module.GetType(), true, typeof(EngineModule))) {
				if (!modulesByType.TryGetValue(type, out var list)) {
					modulesByType[type] = list = new List<EngineModule>();
				}

				list.Add(module);
			}

			if (modulesReady) {
				RefreshModules();
			}
		}

		public bool TryGetModule<T>(out T result) where T : EngineModule
		{
			for (int i = 0; i < modules.Count; i++) {
				if (modules[i] is T t) {
					result = t;

					return true;
				}
			}

			result = default;

			return false;
		}

		public bool TryGetModule(Type type, out EngineModule result)
		{
			if (modulesByType == null || !modulesByType.TryGetValue(type, out var list)) {
				result = default;

				return false;
			}

			result = list[0];

			return true;
		}

		public T GetModule<T>(bool throwOnFailure = true) where T : EngineModule
		{
			if (TryGetModule<T>(out var result)) {
				return result;
			}

			if (!throwOnFailure) {
				return null;
			}

			throw new ArgumentException($"The current {nameof(Game)} instance does not contain a '{typeof(T).Name}' engine module.");
		}

		public EngineModule GetModule(Type type, bool throwOnFailure = true)
		{
			if (TryGetModule(type, out var result)) {
				return result;
			}

			if (!throwOnFailure) {
				return null;
			}

			throw new ArgumentException($"The current {nameof(Game)} instance does not contain a '{type.Name}' engine module.");
		}

		private void InitializeModules()
		{
			lock(AssemblyCache.EngineTypes) {
				foreach (var type in AssemblyCache.EngineTypes.Where(t => !t.IsAbstract && typeof(EngineModule).IsAssignableFrom(t))) {
					if (AutoloadAttribute.TypeNeedsAutoloading(type)) {
						AddModule((EngineModule)Activator.CreateInstance(type));
					}
				}
			}

			RefreshModules();

			modulesReady = true;
		}

		private void RefreshModules()
		{
			SortModules();
			RebuildModuleHooks();
		}

		private void SortModules()
		{
			IEnumerable<EngineModule> GetDirectDependencies(EngineModule module)
				=> module.Dependencies?.Select(dependency => {
					if (!TryGetModule(dependency.Type, out var result) && !dependency.Optional) {
						throw new Exception($"Unable to find module of type '{dependency.Type.Name}', required by module '{module.GetType().Name}'.");
					}

					return result;
				});

			modules = modules.DependencySort(GetDirectDependencies, true).ToList();

			for (int i = 0; i < modules.Count; i++) {
				modules[i].DependencyIndex = i;
			}
		}

		private void RebuildModuleHooks()
		{
			static int CustomSorting((EngineModule module, Delegate method, int position) tupleA, (EngineModule module, Delegate method, int position) tupleB)
				=> tupleA.module.DependencyIndex < tupleB.module.DependencyIndex && tupleA.position < tupleB.position ? -1 : 1;

			HookUtils.BuildHooksFromVirtualMethods(modules, moduleHooks, customSorting: CustomSorting);
		}
	}
}
