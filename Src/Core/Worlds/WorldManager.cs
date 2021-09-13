using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public sealed class WorldManager : EngineModule
	{
		public static World DefaultWorld { get; private set; }

		internal static event Action<World> OnWorldCreated;
		internal static event Action<World> OnWorldDestroyed;

		private static readonly List<World> Worlds = new();

		protected override void Init()
		{
			DefaultWorld = CreateWorld();
		}

		public static World CreateWorld()
		{
			var world = new World(Worlds.Count);

			Worlds.Add(world);

			OnWorldCreated?.Invoke(world);

			return world;
		}

		public static World GetWorld(int id) => Worlds[id] ?? throw new ArgumentException($"No world with id '{id}'.");

		public static bool TryGetWorld(int id, out World result)
		{
			if (id >= 0 && id < Worlds.Count) {
				result = Worlds[id];

				return result != null;
			}

			result = null;

			return false;
		}

		internal static ReadOnlySpan<World> ReadWorlds() => CollectionsMarshal.AsSpan(Worlds);
	}
}
