using System;
using System.Collections.Generic;
using System.Linq;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public class SystemsManager : EngineModule
	{
		private static readonly Dictionary<Type, SystemBase> SystemsByType = new Dictionary<Type, SystemBase>();

		private static List<SystemBase> systems = new List<SystemBase>();

		protected override void PreInit()
		{
			AssemblyRegistrationModule.OnAssemblyRegistered += (assembly, types) => {
				foreach(var type in types) {
					if(type.IsAbstract || !typeof(SystemBase).IsAssignableFrom(type)) {
						continue;
					}

					var system = (SystemBase)Activator.CreateInstance(type);

					systems.Add(system);
					SystemsByType.Add(type, system);
				}

				SortSystems();
			};
		}
		protected override void FixedUpdate()
		{
			foreach(var system in systems) {
				system.FixedUpdate();
			}
		}
		protected override void RenderUpdate()
		{
			foreach(var system in systems) {
				system.RenderUpdate();
			}
		}

		private void SortSystems()
		{
			IEnumerable<SystemBase> GetDirectDependencies(SystemBase system)
				=> system.Dependencies?.Select(dependency => {
					if(!SystemsByType.TryGetValue(dependency.type, out var result) && !dependency.optional) {
						throw new Exception($"Unable to find module of type '{dependency.type.Name}', required by module '{system.GetType().Name}'.");
					}

					return result;
				});

			systems = systems.DependencySort(GetDirectDependencies, true).ToList();

			/*for(int i = 0; i < Systems.Count; i++) {
				Systems[i].DependencyIndex = i;
			}*/
		}
	}
}
