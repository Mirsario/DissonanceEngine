using System;
using System.Collections.Generic;

namespace Dissonance.Engine
{
	public class SystemsManager : EngineModule
	{
		//private static readonly Dictionary<Type, GameSystem> GameSystemsByType = new Dictionary<Type, GameSystem>();
		//private static readonly Dictionary<Type, RenderSystem> RenderSystemsByType = new Dictionary<Type, RenderSystem>();

		internal static readonly List<Type> GameSystemTypes = new();
		internal static readonly List<Type> RenderSystemTypes = new();
		//private static List<GameSystem> gameSystems = new List<GameSystem>();
		//private static List<RenderSystem> renderSystems = new List<RenderSystem>();

		protected override void PreInit()
		{
			AssemblyRegistrationModule.OnAssemblyRegistered += (assembly, types) => {
				foreach(var type in types) {
					if(type.IsAbstract || !typeof(SystemBase).IsAssignableFrom(type)) {
						continue;
					}

					if(typeof(GameSystem).IsAssignableFrom(type)) {
						GameSystemTypes.Add(type);
					} else if(typeof(RenderSystem).IsAssignableFrom(type)) {
						RenderSystemTypes.Add(type);
					}
				}

				//SortSystems();
			};
		}
		/*protected override void FixedUpdate()
		{
			foreach(var system in gameSystems) {
				system.Update();
			}
		}
		protected override void RenderUpdate()
		{
			foreach(var system in renderSystems) {
				system.Update();
			}
		}*/

		/*private void SortSystems()
		{
			IEnumerable<GameSystem> GetGameSystemDependencies(GameSystem system)
				=> system.Dependencies?.Select(dependency => {
					if(!GameSystemsByType.TryGetValue(dependency.type, out var result) && !dependency.optional) {
						throw new Exception($"Unable to find module of type '{dependency.type.Name}', required by module '{system.GetType().Name}'.");
					}

					return result;
				});

			gameSystems = gameSystems.DependencySort(GetGameSystemDependencies, true).ToList();

			IEnumerable<RenderSystem> GetRenderSystemDependencies(RenderSystem system)
				=> system.Dependencies?.Select(dependency => {
					if(!RenderSystemsByType.TryGetValue(dependency.type, out var result) && !dependency.optional) {
						throw new Exception($"Unable to find module of type '{dependency.type.Name}', required by module '{system.GetType().Name}'.");
					}

					return result;
				});

			renderSystems = renderSystems.DependencySort(GetRenderSystemDependencies, true).ToList();
		}*/

		internal static void AddDefaultSystemsToWorld(World world)
		{
			foreach(var type in GameSystemTypes) {
				var system = (GameSystem)Activator.CreateInstance(type);

				system.World = world;

				system.Initialize();

				world.AddSystem(system);
			}

			foreach(var type in RenderSystemTypes) {
				var system = (RenderSystem)Activator.CreateInstance(type);

				system.World = world;

				system.Initialize();

				world.AddSystem(system);
			}
		}
	}
}
