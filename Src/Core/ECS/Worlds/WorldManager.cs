using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public sealed class WorldManager : EngineModule
	{
		internal const int InvalidtWorldId = 0;
		internal const int DefaultWorldId = 1;

		public static World DefaultWorld { get; private set; }

		internal static event Action<World, WorldCreationOptions> OnWorldCreated;
		internal static event Action<World> OnWorldDestroyed;

		private static readonly List<World> Worlds = new() { null };

		protected override void Init()
		{
			DefaultWorld = CreateWorld();

			// Sanity check
			if (DefaultWorld.Id != DefaultWorldId) {
				throw new InvalidOperationException($"Default world ID equals {DefaultWorld.Id}, but {DefaultWorldId} was expected. Are we all insane here?");
			}
		}

		public static World CreateWorld(WorldCreationOptions? options = null)
		{
			var world = new World(Worlds.Count);
			var usedOptions = options ?? new();

			Worlds.Add(world);

			OnWorldCreated?.Invoke(world, usedOptions);

			world.Init();

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

		internal static ReadOnlySpan<World> ReadWorlds()
		{
			return CollectionsMarshal.AsSpan(Worlds).Slice(1);
		}
	}
}
