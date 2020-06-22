using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.Core.Internal;
using Dissonance.Engine.Core.Modules;
using Dissonance.Engine.Utils.Internal;

namespace Dissonance.Engine.Core
{
	partial class Game
	{
		private List<EngineModule> modules;
		private EngineModuleHooks moduleHooks;

		private void InitializeModules()
		{
			moduleHooks = new EngineModuleHooks();
			modules = new List<EngineModule>();

			lock(ReflectionCache.allTypes) {
				foreach(var type in ReflectionCache.allTypes.Where(t => !t.IsAbstract && typeof(EngineModule).IsAssignableFrom(t))) {
					var instance = (EngineModule)Activator.CreateInstance(type);

					instance.Game = this;
					instance.Dependencies = instance
						.GetType()
						.GetCustomAttributes<ModuleDependencyAttribute>()
						.SelectMany(a => a.Dependencies)
						.ToArray();

					if(instance.AutoLoad) {
						modules.Add(instance);
					}
				}
			}

			IEnumerable<EngineModule> GetDirectDependencies(EngineModule module)
				=> module.Dependencies?.Select(dependency => {
					var result = modules.FirstOrDefault(m => m.GetType()==dependency.type);

					if(result==null && !dependency.optional) {
						throw new Exception($"Unable to find module of type '{dependency.type.Name}', required by module '{module.GetType().Name}'.");
					}

					return result;
				});

			modules = modules.DependencySort(GetDirectDependencies,true).ToList();

			for(int i = 0;i<modules.Count;i++) {
				modules[i].DependencyIndex = i;
			}

			RebuildModuleHooks();
		}
		private void RebuildModuleHooks()
		{
			static int CustomSorting((EngineModule module, Delegate method, int position) tupleA,(EngineModule module, Delegate method, int position) tupleB)
				=> tupleA.module.DependencyIndex<tupleB.module.DependencyIndex && tupleA.position<tupleB.position ? -1 : 1;

			HookUtils.BuildHooksFromVirtualMethods(modules,moduleHooks,customSorting: CustomSorting);
		}
	}
}