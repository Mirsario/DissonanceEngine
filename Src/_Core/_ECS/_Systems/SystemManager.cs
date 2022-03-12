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

		private static readonly Dictionary<Type, SystemTypeData> SystemTypeInfo = new();

		private static readonly List<Type> SystemTypes = new();

		private static WorldData[] worldDataById = Array.Empty<WorldData>();

		protected override void PreInit()
		{
			AssemblyRegistrationModule.OnAssemblyRegistered += OnAssemblyRegistered;

			WorldManager.OnWorldCreated += (world, options) => {
				if (worldDataById.Length <= world.Id) {
					Array.Resize(ref worldDataById, world.Id + 1);
				}

				worldDataById[world.Id] = new();

				AddDefaultSystemsToWorld(world, options.AddDefaultSystems, options.AddDefaultCallbacks);
			};

			WorldManager.OnWorldDestroyed += world => {
				worldDataById[world.Id] = null;

				ArrayUtils.TryShrinking(ref worldDataById);
			};
		}

		protected override void FixedUpdate()
		{
			var worlds = WorldManager.ReadWorlds();

			ExecuteCallbackOnWorlds<BeginFixedUpdateCallback>(worlds);
			ExecuteCallbackOnWorlds<FixedUpdateCallback>(worlds);
			ExecuteCallbackOnWorlds<EndFixedUpdateCallback>(worlds);
		}

		protected override void RenderUpdate()
		{
			var worlds = WorldManager.ReadWorlds();

			ExecuteCallbackOnWorlds<BeginRenderUpdateCallback>(worlds);
			ExecuteCallbackOnWorlds<RenderUpdateCallback>(worlds);
			ExecuteCallbackOnWorlds<EndRenderUpdateCallback>(worlds);
		}

		/// <summary>
		/// Prints a tree of subscribers of the default callback systems on the provided world to the console.
		/// </summary>
		/// <param name="world"> The world to get default callback instances from. </param>
		public static void LogDefaultSystemCallbacksTree(World world)
		{
			Debug.Log($"Configuration of world {world.Id}:");

			var array = new CallbackSystem[] {
				world.GetSystem<BeginFixedUpdateCallback>(),
				world.GetSystem<FixedUpdateCallback>(),
				world.GetSystem<EndFixedUpdateCallback>(),
				world.GetSystem<BeginRenderUpdateCallback>(),
				world.GetSystem<RenderUpdateCallback>(),
				world.GetSystem<EndRenderUpdateCallback>(),
			};

			array = array.Where(s => s != null).ToArray();

			LogSystemCallbacksTree(array);
		}

		/// <summary>
		/// Prints a tree of subscribers of the provided callback systems to the console. Useful when debugging with no editor to help.
		/// </summary>
		/// <param name="callbackSystems"> The callback systems to make trees of. </param>
		public static void LogSystemCallbacksTree(params CallbackSystem[] callbackSystems)
			=> LogSystemCallbacksTree((ReadOnlySpan<CallbackSystem>)callbackSystems);

		/// <inheritdoc cref="LogSystemCallbacksTree(CallbackSystem[])"/>
		public static void LogSystemCallbacksTree(ReadOnlySpan<CallbackSystem> callbackSystems)
		{
			lock (Debug.LoggingLock) {
				for (int i = 0; i < callbackSystems.Length; i++) {
					LogSystemCallbacksTree(callbackSystems[i], string.Empty, i == callbackSystems.Length - 1);
				}
			}
		}

		internal static SystemTypeData GetSystemTypeData<T>() where T : GameSystem
			=> GetSystemTypeData(typeof(T));

		internal static SystemTypeData GetSystemTypeData(Type type)
		{
			if (!SystemTypeInfo.TryGetValue(type, out var typeData)) {
				SystemTypeInfo[type] = typeData = new(type);
			}

			return typeData;
		}

		internal static void AddSystemToWorld<T>(World world) where T : GameSystem
		{
			var system = Activator.CreateInstance<T>();

			AddSystemToWorld(world, system);
		}

		internal static void AddSystemToWorld(World world, GameSystem system)
		{
			var worldData = worldDataById[world.Id];
			var systemType = system.GetType();

			if (!worldData.SystemsByType.TryGetValue(systemType, out var systemsOfThisType)) {
				worldData.SystemsByType[systemType] = systemsOfThisType = new();
			}

			system.World = world;

			// Subscribe this system to its type' callbacks
			foreach (var callbackType in system.TypeData.Callbacks) {
				if (!worldData.SystemsByType.TryGetValue(callbackType, out var callbacksOfThisType)) {
					continue;
				}

				foreach (CallbackSystem callback in callbacksOfThisType) {
					callback.AddSystem(system);
				}
			}

			// Subscribe systems to this one if have it specified in its TypeData
			if (system is CallbackSystem callbackSystem) {
				foreach (var otherSystem in worldData.Systems) {
					if (otherSystem.TypeData.Callbacks.Contains(systemType)) {
						callbackSystem.AddSystem(otherSystem);
					}
				}
			}

			worldData.Systems.Add(system);
			systemsOfThisType.Add(system);
		}

		internal static T GetWorldSystem<T>(World world) where T : GameSystem
		{
			if (TryGetWorldSystem<T>(world, out var result)) {
				return result;
			}

			throw new KeyNotFoundException($"Unable to find any systems of type {typeof(T).Name} on the provided world.");
		}

		internal static bool TryGetWorldSystem<T>(World world, out T result) where T : GameSystem
		{
			var worldData = worldDataById[world.Id];

			if (worldData.SystemsByType.TryGetValue(typeof(T), out var systems) && systems.Count > 0) {
				result = (T)systems[0];

				return true;
			}

			result = null;

			return false;
		}

		internal static void ExecuteCallbacks<T>(World world) where T : CallbackSystem
		{
			var worldData = worldDataById[world.Id];

			if (worldData.SystemsByType.TryGetValue(typeof(T), out var callbacks)) {
				for (int i = 0; i < callbacks.Count; i++) {
					callbacks[i].Update();
				}
			}
		}

		private static void AddDefaultSystemsToWorld(World world, bool addSystems = true, bool addCallbacks = true)
		{
			if (!addCallbacks && !addSystems) {
				return;
			}

			for (int i = 0; i < SystemTypes.Count; i++) {
				var type = SystemTypes[i];
				bool isCallback = typeof(CallbackSystem).IsAssignableFrom(type);

				if (isCallback ? addCallbacks : addSystems) {
					var system = (GameSystem)Activator.CreateInstance(type);

					AddSystemToWorld(world, system);
				}
			}
		}

		private static void OnAssemblyRegistered(Assembly assembly, Type[] types)
		{
			foreach (var type in types) {
				if (type.IsAbstract || !typeof(GameSystem).IsAssignableFrom(type)) {
					continue;
				}

				SystemTypes.Add(type);
			}
		}

		private static void LogSystemCallbacksTree(GameSystem system, string indent, bool last)
		{
			Console.Write(indent);

			if (last) {
				Console.Write("└─");

				indent += "  ";
			} else {
				Console.Write("├─");

				indent += "│ ";
			}

			var systemType = system.GetType();
			bool isEngineSystem = systemType.Assembly == Assembly.GetExecutingAssembly();
			bool isCallback = typeof(CallbackSystem).IsAssignableFrom(systemType);

			Console.ForegroundColor = isEngineSystem
				? (isCallback ? ConsoleColor.DarkGray : ConsoleColor.Gray)
				: (isCallback ? ConsoleColor.DarkGreen : ConsoleColor.Green);

			Console.WriteLine(system.GetType().Name);

			Console.ForegroundColor = ConsoleColor.Gray;

			if (system is CallbackSystem callbackSystem) {
				var enumerator = callbackSystem.InvocationList.GetEnumerator();

				if (enumerator.MoveNext()) {
					bool hasEntries;

					do {
						var entry = enumerator.Current;

						hasEntries = enumerator.MoveNext();

						LogSystemCallbacksTree(entry, indent, !hasEntries);
					}
					while (hasEntries);
				}
			}
		}

		private static void ExecuteCallbackOnWorlds<T>(ReadOnlySpan<World> worlds) where T : CallbackSystem
		{
			foreach (var world in worlds) {
				world.ExecuteCallbacks<T>();
			}
		}
	}
}
