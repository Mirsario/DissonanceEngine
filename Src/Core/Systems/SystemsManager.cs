using System;
using System.Collections.Generic;
using System.Linq;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public class SystemsManager : EngineModule
	{
		internal static readonly Dictionary<Type, SystemTypeInfo> SystemTypeInfo = new();

		private static readonly List<Type> SystemTypes = new();
		private static readonly Dictionary<Type, List<Type>> ComponentTypeToWritingSystemTypes = new();
		private static readonly Dictionary<Type, List<Type>> MessageTypeToSendingSystemTypes = new();

		//private static readonly Dictionary<Type, 

		protected override void PreInit()
		{
			AssemblyRegistrationModule.OnAssemblyRegistered += (assembly, types) => {
				foreach(var type in types) {
					if(!type.IsAbstract && typeof(GameSystem).IsAssignableFrom(type)) {
						SystemTypes.Add(type);

						var systemTypeInfo = new SystemTypeInfo(type);

						//Fill a dictionary that stores, by component type, lists of system types that write that component.
						foreach(var writeType in systemTypeInfo.WriteTypes) {
							if(!ComponentTypeToWritingSystemTypes.TryGetValue(writeType, out var writingSystems)) {
								ComponentTypeToWritingSystemTypes[writeType] = writingSystems = new List<Type>();
							}

							writingSystems.Add(type);
						}

						//Fill a dictionary that stores, by message type, lists of system types that send that message type.
						foreach(var sendType in systemTypeInfo.SendTypes) {
							if(!MessageTypeToSendingSystemTypes.TryGetValue(sendType, out var sendingSystems)) {
								MessageTypeToSendingSystemTypes[sendType] = sendingSystems = new List<Type>();
							}

							sendingSystems.Add(type);
						}

						SystemTypeInfo[type] = systemTypeInfo;
					}
				}
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

		internal static void SortSystems(List<GameSystem> systems, Dictionary<Type, List<GameSystem>> systemsByType)
		{
			IEnumerable<GameSystem> GetDependenciesBasedOnReadTypes(GameSystem system, IEnumerable<Type> readTypes, Dictionary<Type, List<Type>> typeToWritingSystemTypes)
			{
				foreach(var componentType in readTypes) {
					if(!typeToWritingSystemTypes.TryGetValue(componentType, out var writingTypes)) {
						continue;
					}

					foreach(var writingType in writingTypes) {
						if(!systemsByType.TryGetValue(writingType, out var writingSystems)) {
							continue;
						}

						foreach(var writingSystem in writingSystems) {
							if(writingSystem != system) {
								yield return writingSystem;
							}
						}
					}
				}
			}

			IEnumerable<GameSystem> GetGameSystemDependencies(GameSystem system)
			{
				if(system.TypeData.ReadTypes != null) {
					foreach(var result in GetDependenciesBasedOnReadTypes(system, system.TypeData.ReadTypes, ComponentTypeToWritingSystemTypes)) {
						yield return result;
					}
				}

				if(system.TypeData.ReceiveTypes != null) {
					foreach(var result in GetDependenciesBasedOnReadTypes(system, system.TypeData.ReceiveTypes, MessageTypeToSendingSystemTypes)) {
						yield return result;
					}
				}
			}

			var sorted = systems.DependencySort(GetGameSystemDependencies, false).ToArray();

			systems.Clear();
			systems.AddRange(sorted);
		}

		internal static void AddDefaultSystemsToWorld(World world)
		{
			world.DefaultSystemsRegistered = false;

			foreach(var type in SystemTypes) {
				var system = (GameSystem)Activator.CreateInstance(type);

				system.World = world;

				system.Initialize();

				world.AddSystem(system);
			}

			SortSystems(world.Systems, world.SystemsByType);

			world.DefaultSystemsRegistered = true;
		}
	}
}
