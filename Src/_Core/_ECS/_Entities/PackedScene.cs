using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public sealed class PackedScene
	{
		private readonly List<PackedEntity> Entities = new();

		public PackedEntity CreateEntity()
		{
			var entity = new PackedEntity();

			Entities.Add(entity);

			return entity;
		}

		public ReadOnlySpan<PackedEntity> ReadEntities()
		{
			return CollectionsMarshal.AsSpan(Entities);
		}

		public IEnumerable<Entity> Unpack(World world)
		{
			foreach (var entity in Entities) {
				yield return entity.Unpack(world);
			}
		}
	}
}
