using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public sealed class EntitySet
	{
		private readonly List<Entity> Entities = new List<Entity>();

		internal readonly EntitySetType Type;
		internal readonly Predicate<Entity> Predicate;

		internal EntitySet(EntitySetType type, Predicate<Entity> predicate)
		{
			Type = type;
			Predicate = predicate;
		}

		public ReadOnlySpan<Entity> ReadEntities()
		{
			return CollectionsMarshal.AsSpan(Entities);
		}

		internal void OnEntityUpdated(in Entity entity)
		{
			bool result = Predicate(entity);

			if(Entities.Contains(entity) != result) {
				if(result) {
					Entities.Add(entity);
				} else {
					Entities.Remove(entity);
				}
			}
		}
	}
}
