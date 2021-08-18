using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	[ModuleDependency(typeof(WorldManager))]
	public class SystemManager : EngineModule
	{
		private class WorldData
		{
			public readonly List<GameSystem> Systems = new();
			public readonly Dictionary<Type, List<GameSystem>> SystemsByType = new();
		}

		internal static readonly Dictionary<Type, SystemTypeData> SystemTypeInfo = new();

		private static readonly List<Type> SystemTypes = new();
		private static readonly Dictionary<Type, List<Type>> ComponentTypeToWritingSystemTypes = new();
		private static readonly Dictionary<Type, List<Type>> MessageTypeToSendingSystemTypes = new();

		private static WorldData[] worldDataById = Array.Empty<WorldData>();

		protected override void PreInit()
		{
			AssemblyRegistrationModule.OnAssemblyRegistered += OnAssemblyRegistered;

			WorldManager.OnWorldCreated += world => {
				if(worldDataById.Length <= world.Id) {
					Array.Resize(ref worldDataById, world.Id + 1);
				}

				worldDataById[world.Id] = new();

				AddDefaultSystemsToWorld(world);
			};

			WorldManager.OnWorldDestroyed += world => {
				worldDataById[world.Id] = null;

				ArrayUtils.TryShrinking(ref worldDataById);
			};
		}

		protected override void FixedUpdate()
		{
			foreach(var world in WorldManager.ReadWorlds()) {
				var worldData = worldDataById[world.Id];

				foreach(var system in worldData.Systems) {
					if(!system.Initialized) {
						system.Initialize();

						system.Initialized = true;
					}

					system.FixedUpdate();
				}
			}
		}

		protected override void RenderUpdate()
		{
			foreach(var world in WorldManager.ReadWorlds()) {
				var worldData = worldDataById[world.Id];

				foreach(var system in worldData.Systems) {
					system.RenderUpdate();
				}
			}
		}

		internal static void SortSystems(List<GameSystem> systems, Dictionary<Type, List<GameSystem>> systemsByType)
		{
			IEnumerable<GameSystem> GatherDependenciesBasedOnReadTypes(GameSystem system, IEnumerable<Type> readTypes, Dictionary<Type, List<Type>> typeToWritingSystemTypes)
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

			var dependenciesBySystem = new Dictionary<GameSystem, List<GameSystem>>();

			IEnumerable<GameSystem> GetGameSystemDependencies(GameSystem system)
			{
				if(!dependenciesBySystem.TryGetValue(system, out var dependencies)) {
					dependenciesBySystem[system] = dependencies = new();

					var typeData = system.TypeData;

					if(typeData.ReadTypes?.Count > 0) {
						// Add systems that write/modify component values as dependencies of systems that read them.
						dependencies.AddRange(GatherDependenciesBasedOnReadTypes(system, typeData.ReadTypes, ComponentTypeToWritingSystemTypes));
					}

					if(typeData.ReceiveTypes?.Count > 0) {
						// Add systems that send messages as dependencies of systems that receive them.
						dependencies.AddRange(GatherDependenciesBasedOnReadTypes(system, typeData.ReceiveTypes, MessageTypeToSendingSystemTypes));
					}
				}

				return dependencies;
			}

			var sorted = systems.DependencySort(GetGameSystemDependencies, false).ToArray();

#if DEBUG
			Debug.Log($"System order: \r\n{string.Join("\r\n", sorted.Select(s => s.GetType().Name))}");
#endif

			systems.Clear();
			systems.AddRange(sorted);
		}

		internal static void AddSystemToWorld(World world, GameSystem system, bool sortSystems = true)
		{
			var worldData = worldDataById[world.Id];
			var systemType = system.GetType();

			if(!worldData.SystemsByType.TryGetValue(systemType, out var systemsOfThisType)) {
				worldData.SystemsByType[systemType] = systemsOfThisType = new();
			}

			worldData.Systems.Add(system);
			systemsOfThisType.Add(system);

			if(sortSystems) {
				SortSystems(worldData.Systems, worldData.SystemsByType);
			}
		}

		private static void AddDefaultSystemsToWorld(World world)
		{
			for(int i = 0; i < SystemTypes.Count; i++) {
				var type = SystemTypes[i];
				var system = (GameSystem)Activator.CreateInstance(type);

				system.World = world;

				AddSystemToWorld(world, system, sortSystems: i == SystemTypes.Count - 1);
			}
		}

		private static void OnAssemblyRegistered(Assembly assembly, Type[] types)
		{
			foreach(var type in types) {
				if(type.IsAbstract || !typeof(GameSystem).IsAssignableFrom(type)) {
					continue;
				}

				SystemTypes.Add(type);

				var systemTypeInfo = new SystemTypeData(type);

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
	}
}
