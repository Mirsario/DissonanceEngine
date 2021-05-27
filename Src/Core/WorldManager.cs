using System;
using System.Collections.Generic;
using System.Text;

namespace Dissonance.Engine
{
	public sealed class WorldManager : EngineModule
	{
		private static readonly List<World> Worlds = new List<World>();
		private static readonly IReadOnlyList<World> WorldsReadOnly = new List<World>();

		static WorldManager()
		{

		}

		protected override void FixedUpdate()
		{
			foreach(var world in Worlds) {
				world.FixedUpdate();
			}
		}
		protected override void RenderUpdate()
		{
			foreach(var world in Worlds) {
				world.RenderUpdate();
			}
		}

		public static World CreateWorld()
		{
			var world = new World(Worlds.Count);

			SystemsManager.AddDefaultSystemsToWorld(world);

			Worlds.Add(world);

			return world;
		}

		public static World GetWorld(int id)
		{
			return Worlds[id];
		}

		public static IEnumerable<World> EnumerateWorlds()
		{
			return WorldsReadOnly;
		}
	}
}
