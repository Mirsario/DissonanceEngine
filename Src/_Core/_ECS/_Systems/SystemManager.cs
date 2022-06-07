using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine;

[ModuleDependency<WorldManager>]
public class SystemManager : EngineModule
{
	private class WorldData
	{
		public readonly List<GameSystem> Systems = new();
		public readonly Dictionary<Type, GameSystem> SystemsByType = new();
	}

	private static readonly List<GameSystem> systems = new();
	private static readonly Dictionary<Type, GameSystem> systemsByType = new();
	private static readonly Dictionary<Type, SystemTypeData> systemTypeInfo = new();

	private static WorldData[] worldDataById = Array.Empty<WorldData>();

	protected override void PreInit()
	{
		WorldManager.OnWorldCreated += (world, options) => {
			if (worldDataById.Length <= world.Id) {
				Array.Resize(ref worldDataById, world.Id + 1);
			}

			worldDataById[world.Id] = new();

			//AddDefaultSystemsToWorld(world, options.AddDefaultSystems, options.AddDefaultCallbacks);
		};

		WorldManager.OnWorldDestroyed += world => {
			worldDataById[world.Id] = null;

			ArrayUtils.TryShrinking(ref worldDataById);
		};
	}

	protected override void Init()
	{
		for (int i = 0; i < systems.Count; i++) {
			var system = systems[i];
			var systemType = system.GetType();

			// Subscribe this system to its type's callbacks
			foreach (var callbackType in system.TypeData.Callbacks) {
				if (systemsByType.TryGetValue(callbackType, out var callback)) {
					((CallbackSystem)callback).AddSystem(system);
				}
			}

			// Subscribe systems to this one if they have it specified in its TypeData
			/*
			if (system is CallbackSystem callbackSystem) {
				foreach (var otherSystem in systems) {
					if (otherSystem.TypeData.Callbacks.Contains(systemType)) {
						callbackSystem.AddSystem(otherSystem);
					}
				}
			}
			*/
		}
	}

	protected override void InitializeForAssembly(Assembly assembly)
	{
		foreach (var type in assembly.GetTypes()) {
			if (type.IsAbstract || !typeof(GameSystem).IsAssignableFrom(type)) {
				continue;
			}

			if (!AutoloadAttribute.TypeNeedsAutoloading(type)) {
				continue;
			}

			var system = (GameSystem)Activator.CreateInstance(type);

			systems.Add(system);
			systemsByType.Add(type, system);
		}
	}

	protected override void FixedUpdate()
	{
		//ExecuteCallback<RootFixedUpdateCallback>();
		ExecuteCallback<BeginFixedUpdateCallback>();
		ExecuteCallback<FixedUpdateCallback>();
		ExecuteCallback<EndFixedUpdateCallback>();
	}

	protected override void RenderUpdate()
	{
		///ExecuteCallback<RootRenderUpdateCallback>();
		ExecuteCallback<BeginRenderUpdateCallback>();
		ExecuteCallback<RenderUpdateCallback>();
		ExecuteCallback<EndRenderUpdateCallback>();
	}

	/// <summary>
	/// Prints a tree of subscribers of the default callback systems on the provided world to the console.
	/// </summary>
	/// <param name="world"> The world to get default callback instances from. </param>
	public static void LogDefaultSystemCallbacksTree(/*World world*/)
	{
		/*
		Debug.Log($"Configuration of world {world.Id}:");

		var array = new CallbackSystem[] {
			world.GetSystem<BeginFixedUpdateCallback>(),
			world.GetSystem<FixedUpdateCallback>(),
			world.GetSystem<EndFixedUpdateCallback>(),
			world.GetSystem<BeginRenderUpdateCallback>(),
			world.GetSystem<RenderUpdateCallback>(),
			world.GetSystem<EndRenderUpdateCallback>(),
		};
		*/
		
		var array = new CallbackSystem[] {
			GetSystem<BeginFixedUpdateCallback>(),
			GetSystem<FixedUpdateCallback>(),
			GetSystem<EndFixedUpdateCallback>(),
			GetSystem<BeginRenderUpdateCallback>(),
			GetSystem<RenderUpdateCallback>(),
			GetSystem<EndRenderUpdateCallback>(),
			GetSystem<Graphics.RenderingCallback>(),
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

	public static IEnumerable<SystemTypeData> EnumerateSystemTypes()
	{
		return systemTypeInfo.Values;
	}

	public static T GetSystem<T>() where T : GameSystem
		=> TryGetSystem<T>(out var result) ? result : throw new InvalidOperationException($"No system of type {typeof(T).Name} exists.");

	public static bool TryGetSystem<T>(out T result) where T : GameSystem
	{
		if (systemsByType.TryGetValue(typeof(T), out var system)) {
			result = (T)system;

			return true;
		}

		result = default;

		return false;
	}

	internal static SystemTypeData GetSystemTypeData<T>() where T : GameSystem
		=> GetSystemTypeData(typeof(T));

	internal static SystemTypeData GetSystemTypeData(Type type)
	{
		if (!systemTypeInfo.TryGetValue(type, out var typeData)) {
			systemTypeInfo[type] = typeData = new(type);
		}

		return typeData;
	}

	internal static void ExecuteCallback<T>() where T : CallbackSystem
	{
		if (!TryGetSystem(out T callbackSystem)) {
			throw new InvalidOperationException($"Unable to execute callbacks of type {typeof(T).Name} because it is not registered.");
		}

		callbackSystem.Update();
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
}
