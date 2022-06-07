using System.Collections.Generic;

namespace Dissonance.Engine;

public sealed class EntitySet
{
	private readonly List<Entity> Entities = new();

	internal readonly ComponentSet ComponentSet;
	internal readonly bool? EntityIsActiveFilter;

	internal EntitySet(ComponentSet componentSet, bool? entityIsActiveFilter = true)
	{
		ComponentSet = componentSet;
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
		bool shouldContain = (!EntityIsActiveFilter.HasValue || EntityIsActiveFilter.Value == entity.IsActive) && ComponentSet.Matches(entity);

		if (contains != shouldContain) {
			if (shouldContain) {
				Entities.Add(entity);
			} else {
				Entities.RemoveAt(index);
			}
		}
	}
}
