using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public sealed class EntityManager : EngineModule
	{
		private class WorldData
		{
			// Entities
			public readonly List<Entity> Entities = new();
			public readonly List<Entity> ActiveEntities = new();
			public readonly List<Entity> InactiveEntities = new();
			public readonly List<int> FreeEntityIndices = new();
			public readonly List<bool> EntityIsActive = new(); //TODO: Replace with BitArray, or a wrapping type.
			// Entity Sets
			public readonly List<EntitySet> EntitySets = new();
			public readonly Dictionary<Expression<Predicate<Entity>>, EntitySet> EntitySetByExpression = new();
		}

		private static WorldData[] worldDataById = Array.Empty<WorldData>();

		protected override void PreInit()
		{
			WorldManager.OnWorldCreated += OnWorldCreated;
			WorldManager.OnWorldDestroyed += OnWorldDestroyed;
			ComponentManager.OnComponentAdded += OnComponentPresenceModified;
			ComponentManager.OnComponentRemoved += OnComponentPresenceModified;
		}

		protected override void OnDispose()
		{
			WorldManager.OnWorldCreated -= OnWorldCreated;
			WorldManager.OnWorldDestroyed -= OnWorldDestroyed;
			ComponentManager.OnComponentAdded -= OnComponentPresenceModified;
			ComponentManager.OnComponentRemoved -= OnComponentPresenceModified;
		}

		internal static Entity CreateEntity(int worldId)
		{
			var worldData = worldDataById[worldId];

			int id;

			if (worldData.FreeEntityIndices.Count > 0) {
				id = worldData.FreeEntityIndices[0];
				worldData.FreeEntityIndices.RemoveAt(0);
			} else {
				id = worldData.Entities.Count;
			}

			var entity = new Entity(id, worldId);

			if (id >= worldData.Entities.Count) {
				worldData.Entities.Add(entity);
				worldData.EntityIsActive.Add(true);
			} else {
				worldData.Entities[id] = entity;
				worldData.EntityIsActive[id] = true;
			}

			worldData.ActiveEntities.Add(entity);

			return entity;
		}

		internal static bool RemoveEntity(in Entity entity)
		{
			if (entity.WorldId >= 0 && entity.WorldId < worldDataById.Length) {
				var worldData = worldDataById[entity.WorldId];

				if (worldData != null) {
					worldData.Entities.Remove(entity);

					return true;
				}
			}

			return false;
		}

		internal static bool GetEntityIsActive(in Entity entity)
			=> worldDataById[entity.WorldId].EntityIsActive[entity.Id];

		internal static void SetEntityIsActive(in Entity entity, bool value)
		{
			var worldData = worldDataById[entity.WorldId];
			bool isActive = worldData.EntityIsActive[entity.Id];

			if (value != isActive) {
				if (value) {
					worldData.InactiveEntities.Remove(entity);
					worldData.ActiveEntities.Add(entity);
				} else {
					worldData.ActiveEntities.Remove(entity);
					worldData.InactiveEntities.Add(entity);
				}

				worldData.EntityIsActive[entity.Id] = value;

				UpdateEntitySets(entity);
			}
		}

		internal static ReadOnlySpan<Entity> ReadEntities(int worldId, bool? active = true)
		{
			var worldData = worldDataById[worldId];

			return CollectionsMarshal.AsSpan(active.HasValue ? (active.Value ? worldData.ActiveEntities : worldData.InactiveEntities) : worldData.Entities);
		}

		internal static EntitySet GetEntitySet(int worldId, Expression<Predicate<Entity>> predicate)
		{
			var worldData = worldDataById[worldId];
			var entitySetByExpression = worldData.EntitySetByExpression;

			//TODO: When this fails, check if there are other expressions which are basically the same
			if (entitySetByExpression.TryGetValue(predicate, out var result)) {
				return result;
			}

			var entitySet = new EntitySet(predicate.Compile());

			//TODO: Be smarter about this. Deconstruct expressions, enumerate only entities that contain the least-common component.
			foreach (var entity in worldData.Entities) {
				entitySet.OnEntityUpdated(entity);
			}

			entitySetByExpression[predicate] = entitySet;

			worldData.EntitySets.Add(entitySet);

			return entitySet;
		}

		private static void OnWorldCreated(World world)
		{
			if (worldDataById.Length <= world.Id) {
				Array.Resize(ref worldDataById, world.Id + 1);
			}

			worldDataById[world.Id] = new();
		}

		private static void OnWorldDestroyed(World world)
		{
			worldDataById[world.Id] = null;
		}

		private static void OnComponentPresenceModified(Entity entity, Type componentType)
			=> UpdateEntitySets(entity);

		private static void UpdateEntitySets(in Entity entity)
		{
			var worldData = worldDataById[entity.WorldId];

			foreach (var entitySet in worldData.EntitySets) {
				entitySet.OnEntityUpdated(entity);
			}
		}
	}
}
