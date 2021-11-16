﻿using System;
using System.Collections.Generic;

namespace Dissonance.Engine
{
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
			bool result = (!EntityIsActiveFilter.HasValue || EntityIsActiveFilter.Value == entity.IsActive) && Predicate(entity);

			if (Entities.Contains(entity) != result) {
				if (result) {
					Entities.Add(entity);
				} else {
					Entities.Remove(entity);
				}
			}
		}
	}
}
