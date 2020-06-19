using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.Core.Modules;
using Dissonance.Engine.Utils.Internal;

namespace Dissonance.Engine
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

					if(instance.AutoLoad) {
						modules.Add(instance);
					}
				}
			}

			RebuildModuleHooks();
		}
		private void RebuildModuleHooks() => HookUtils.BuildHooksFromVirtualMethods(modules,moduleHooks);
	}
}