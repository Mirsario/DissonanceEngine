using System;
using System.Collections.Generic;

namespace Dissonance.Engine;

public sealed class EntitySet
{
	private readonly List<Entity> Entities = new();

	internal readonly Predicate<Entity> Predicate;
	internal readonly bool? EntityIsActiveFilter;

	internal EntitySet(Predicate<Entity> predicate, bool? entityIsActiveFilter = true)
	{
		Predicate = predicate;
		EntityIsActiveFilter = entityIsActiveFilter;
	}

	public EntitySetEnumerator ReadEntities()
	{
		return new EntitySetEnumerator(Entities);
	}

	internal void OnEntityUpdated(in Entity entity)
	{
		int index = Entities.IndexOf(entity);
		bool contains = index != -1;
		bool shouldContain = (!EntityIsActiveFilter.HasValue || EntityIsActiveFilter.Value == entity.IsActive) && Predicate(entity);

		if (contains != shouldContain) {
			if (shouldContain) {
				Entities.Add(entity);
			} else {
				Entities.RemoveAt(index);
			}
		}
	}
}
