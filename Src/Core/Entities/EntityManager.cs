using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public sealed class EntityManager : EngineModule
	{
		private static List<Entity> allEntities = new();

		internal static Entity CreateEntity(World world)
		{
			int id;

			if(world.FreeEntityIndices.Count > 0) {
				id = world.FreeEntityIndices[0];
				world.FreeEntityIndices.RemoveAt(0);
			} else {
				id = world.Entities.Count;
			}

			var entity = new Entity(id, world.Id);

			if(id >= world.Entities.Count) {
				world.Entities.Add(entity);
			} else {
				world.Entities[id] = entity;
			}

			allEntities.Add(entity);

			return entity;
		}

		public static ReadOnlySpan<Entity> ReadAllEntities()
		{
			return CollectionsMarshal.AsSpan(allEntities);
		}
	}
}
