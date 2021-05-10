using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Dissonance.Engine
{
	public sealed class EntityManager : EngineModule
	{
		private static readonly List<Entity> Entities = new List<Entity>();
		private static readonly List<int> FreeEntityIndices = new List<int>();
		
		public static Entity CreateEntity()
		{
			int id;

			if(FreeEntityIndices.Count > 0) {
				id = FreeEntityIndices[0];
				FreeEntityIndices.RemoveAt(0);
			} else {
				id = Entities.Count;
			}

			var entity = new Entity(id);

			if(id >= Entities.Count) {
				Entities.Add(entity);
			} else {
				Entities[id] = entity;
			}

			return entity;
		}

		public static IEnumerable<Entity> EnumerateEntities()
		{
			return Entities;
		}
	}
}
