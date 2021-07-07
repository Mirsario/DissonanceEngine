using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public sealed class WorldManager : EngineModule
	{
		internal static event Action<World> OnWorldCreated;
		internal static event Action<World> OnWorldDestroyed;

		private static readonly List<World> Worlds = new List<World>();

		public static World CreateWorld()
		{
			var world = new World(Worlds.Count);

			Worlds.Add(world);

			OnWorldCreated?.Invoke(world);

			return world;
		}

		public static World GetWorld(int id) => Worlds[id];

		internal static ReadOnlySpan<World> ReadWorlds() => CollectionsMarshal.AsSpan(Worlds);
	}
}
