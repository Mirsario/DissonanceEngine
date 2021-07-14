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
			//Entities
			public readonly List<Entity> Entities = new();
			public readonly List<Entity> ActiveEntities = new();
			public readonly List<Entity> InactiveEntities = new();
			public readonly List<int> FreeEntityIndices = new();
			public readonly List<bool> EntityIsActive = new(); //TODO: Replace with BitArray, or a wrapping type.
			//Entity Sets
			public readonly List<EntitySet> EntitySets = new();
			public readonly Dictionary<EntitySetType, Dictionary<Expression<Predicate<Entity>>, EntitySet>> EntitySetsQuery = new();
		}

		private static WorldData[] worldDataById = Array.Empty<WorldData>();

		protected override void PreInit()
		{
			WorldManager.OnWorldCreated += OnWorldCreated;
			WorldManager.OnWorldDestroyed += OnWorldDestroyed;
			ComponentManager.OnComponentAdded += OnComponentAdded;
		}

		protected override void OnDispose()
		{
			WorldManager.OnWorldCreated -= OnWorldCreated;
			WorldManager.OnWorldDestroyed -= OnWorldDestroyed;
			ComponentManager.OnComponentAdded -= OnComponentAdded;
		}

		internal static Entity CreateEntity(int worldId)
		{
			var worldData = worldDataById[worldId];

			int id;

			if(worldData.FreeEntityIndices.Count > 0) {
				id = worldData.FreeEntityIndices[0];
				worldData.FreeEntityIndices.RemoveAt(0);
			} else {
				id = worldData.Entities.Count;
			}

			var entity = new Entity(id, worldId);

			if(id >= worldData.Entities.Count) {
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
			if(entity.WorldId >= 0 && entity.WorldId < worldDataById.Length) {
				var worldData = worldDataById[entity.WorldId];

				if(worldData != null) {
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

			if(value != isActive) {
				if(value) {
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

		internal static EntitySet GetEntitySet(int worldId, EntitySetType type, Expression<Predicate<Entity>> predicate)
		{
			var worldData = worldDataById[worldId];

			if(!worldData.EntitySetsQuery.TryGetValue(type, out var setsByExpressions)) {
				worldData.EntitySetsQuery[type] = setsByExpressions = new();
			} else {
				//TODO: It seems like Expression.ToString() doesn't at all care about method type parameters.
				/*string expressionString = predicate.ToString();

				foreach(var pair in setsByExpressions) {
					var expression = pair.Key;

					//TODO: Make a more elaborate comparer for expressions. This check has a lot of issues, like variable names mattering.
					if(expression.ToString() == expressionString) {
						return pair.Value;
					}
				}*/
			}

			var entitySet = new EntitySet(type, predicate.Compile());

			foreach(var entity in worldData.Entities) {
				entitySet.OnEntityUpdated(entity);
			}

			setsByExpressions[predicate] = entitySet;

			worldData.EntitySets.Add(entitySet);

			return entitySet;
		}

		private static void OnWorldCreated(World world)
		{
			if(worldDataById.Length <= world.Id) {
				Array.Resize(ref worldDataById, world.Id + 1);
			}

			worldDataById[world.Id] = new();
		}

		private static void OnWorldDestroyed(World world)
		{
			worldDataById[world.Id] = null;
		}

		private static void OnComponentAdded(Entity entity, Type componentType)
			=> UpdateEntitySets(entity);

		private static void UpdateEntitySets(in Entity entity)
		{
			var worldData = worldDataById[entity.WorldId];

			foreach(var entitySet in worldData.EntitySets) {
				entitySet.OnEntityUpdated(entity);
			}
		}
	}
}
