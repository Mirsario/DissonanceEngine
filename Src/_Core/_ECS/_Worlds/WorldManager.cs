using System;
using System.Collections.Generic;

namespace Dissonance.Engine;

public sealed class WorldManager : EngineModule
{
	internal const int InvalidWorldId = 0;
	internal const int PrefabWorldId = 1;
	internal const int DefaultWorldId = 2;

	public static World DefaultWorld { get; private set; }
	public static World PrefabWorld { get; private set; }

	internal static readonly List<WorldSet> worldSets = new();
	internal static readonly Dictionary<ComponentSet, WorldSet> worldSetsByComponentSets = new();

	private static readonly List<World> Worlds = new() { null };

	internal static event Action<World, WorldCreationOptions> OnWorldCreated;
	internal static event Action<World> OnWorldDestroyed; //TODO: Call this.

	protected override void Init()
	{
		PrefabWorld = CreateWorld(new WorldCreationOptions { AddDefaultSystems = false });
		DefaultWorld = CreateWorld();

		// Sanity check
		if (DefaultWorld.Id != DefaultWorldId) {
			throw new InvalidOperationException($"Default world ID equals {DefaultWorld.Id}, but {DefaultWorldId} was expected. Are we all insane here?");
		}
	}

	public static World CreateWorld(WorldCreationOptions? options = null)
	{
		var usedOptions = options ?? new();
		var world = new World(Worlds.Count, usedOptions);

		Worlds.Add(world);

		OnWorldCreated?.Invoke(world, usedOptions);

		return world;
	}

	public static World GetWorld(int id)
		=> Worlds[id] ?? throw new ArgumentException($"No world with id '{id}'.");

	public static bool TryGetWorld(int id, out World result)
	{
		if (id >= 0 && id < Worlds.Count) {
			result = Worlds[id];

			return result != null;
		}

		result = null;

		return false;
	}

	public static WorldEnumerator ReadWorlds()
	{
		return new WorldEnumerator(Worlds, 1);
	}

	public static WorldSet GetWorldSet(ComponentSet componentSet)
	{
		if (worldSetsByComponentSets.TryGetValue(componentSet, out var result)) {
			return result;
		}

		var worldSet = new WorldSet(componentSet);

		foreach (var world in ReadWorlds()) {
			worldSet.OnWorldUpdated(world);
		}

		worldSetsByComponentSets[componentSet] = worldSet;

		worldSets.Add(worldSet);

		return worldSet;
	}
}
