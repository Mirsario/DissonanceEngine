using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.Core.Attributes;
using Dissonance.Engine.Core.Modules;
using Dissonance.Engine.Utils;
using Dissonance.Engine.Utils.Internal;

namespace Dissonance.Engine.Core
{
	partial class Game
	{
		private bool modulesReady;
		private List<EngineModule> modules;
		private Dictionary<Type, List<EngineModule>> modulesByType;
		private EngineModuleHooks moduleHooks;

		public void AddModule(EngineModule module)
		{
			modules.Add(module);

			foreach(var type in ReflectionUtils.EnumerateBaseTypes(module.GetType(), true, typeof(EngineModule))) {
				if(!modulesByType.TryGetValue(type, out var list)) {
					modulesByType[type] = list = new List<EngineModule>();
				}

				list.Add(module);
			}

			if(modulesReady) {
				RefreshModules();
			}
		}
		public bool TryGetModule<T>(out T result) where T : EngineModule
		{
			if(TryGetModule(typeof(T), out var tempResult)) {
				result = (T)tempResult;

				return true;
			}

			result = default;

			return false;
		}
		public bool TryGetModule(Type type, out EngineModule result)
		{
			if(modulesByType == null || !modulesByType.TryGetValue(type, out var list)) {
				result = default;

				return false;
			}

			result = list[0];

			return true;
		}
		public T GetModule<T>(bool throwOnFailure = true) where T : EngineModule
			=> (T)GetModule(typeof(T), throwOnFailure);
		public EngineModule GetModule(Type type, bool throwOnFailure = true)
		{
			if(TryGetModule(type, out var result)) {
				return result;
			}

			if(!throwOnFailure) {
				return null;
			}

			throw new ArgumentException($"The current {nameof(Game)} instance does not contain a '{type.Name}' engine module.");
		}

		private void InitializeModules()
		{
			moduleHooks = new EngineModuleHooks();
			modules = new List<EngineModule>();
			modulesByType = new Dictionary<Type, List<EngineModule>>();

			lock(AssemblyCache.AllTypes) {
				foreach(var type in AssemblyCache.AllTypes.Where(t => !t.IsAbstract && typeof(EngineModule).IsAssignableFrom(t))) {
					var autoload = AutoloadAttribute.Get(type);

					if(!autoload.NeedsAutoloading) {
						continue;
					}

					var instance = (EngineModule)Activator.CreateInstance(type);

					instance.Game = this;
					instance.Dependencies = instance
						.GetType()
						.GetCustomAttributes<ModuleDependencyAttribute>()
						.SelectMany(a => a.Dependencies)
						.ToArray();

					AddModule(instance);
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
					if(!TryGetModule(dependency.type, out var result) && !dependency.optional) {
						throw new Exception($"Unable to find module of type '{dependency.type.Name}', required by module '{module.GetType().Name}'.");
					}

					return result;
				});

			modules = modules.DependencySort(GetDirectDependencies, true).ToList();

			for(int i = 0; i < modules.Count; i++) {
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
