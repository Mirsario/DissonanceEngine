using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public sealed class EntityManager : EngineModule
	{
		//TODO: Reduce memory usage later?
		private struct EntityData
		{
			public bool IsActive = default;
			public List<int> PresentComponentTypes = new();

			public EntityData() { }
		}

		private class WorldData
		{
			// Entities
			public readonly List<int> AllEntityIds = new();
			public readonly List<int> ActiveEntityIds = new();
			public readonly List<int> InactiveEntityIds = new();
			public readonly ConcurrentBag<int> FreeEntityIndices = new();
			public EntityData[] EntityData = Array.Empty<EntityData>();
			public int NextEntityIndex;
			// Entity Sets
			public readonly List<EntitySet> EntitySets = new();
			public readonly Dictionary<Expression<Predicate<Entity>>, EntitySet> EntitySetByExpression = new();

			public WorldData() { }
		}

		private static readonly object lockObject = new();

		private static WorldData[] worldDataById = Array.Empty<WorldData>();

		protected override void PreInit()
		{
			WorldManager.OnWorldCreated += OnWorldCreated;
			WorldManager.OnWorldDestroyed += OnWorldDestroyed;
		}

		protected override void OnDispose()
		{
			WorldManager.OnWorldCreated -= OnWorldCreated;
			WorldManager.OnWorldDestroyed -= OnWorldDestroyed;
		}

		internal static Entity CreateEntity(int worldId, bool activate)
		{
			var worldData = worldDataById[worldId];

			int id;

			if (worldData.FreeEntityIndices.TryTake(out int freeIndex)) {
				id = freeIndex;
			} else {
				id = worldData.NextEntityIndex++;
			}

			var entity = new Entity(id, worldId);

			lock (lockObject) {
				if (id >= worldData.EntityData.Length) {
					int newSize = Math.Max(1, worldData.EntityData.Length);

					while (newSize <= id) {
						newSize *= 2;
					}

					Array.Resize(ref worldData.EntityData, newSize);
				}

				worldData.EntityData[id] = new EntityData {
					IsActive = activate
				};
			}

			worldData.AllEntityIds.Add(id);

			if (activate) {
				worldData.ActiveEntityIds.Add(id);
			}

			return entity;
		}

		internal static bool RemoveEntity(int worldId, int entityId)
		{
			if (worldId >= 0 && worldId < worldDataById.Length) {
				var worldData = worldDataById[worldId];

				if (worldData != null) {
					if (worldData.AllEntityIds.Remove(entityId)) {
						worldData.ActiveEntityIds.Remove(entityId);
						worldData.InactiveEntityIds.Remove(entityId);

						ref var entityData = ref worldData.EntityData[entityId];

						lock (entityData.PresentComponentTypes) {
							foreach (int componentId in CollectionsMarshal.AsSpan(entityData.PresentComponentTypes)) {
								ComponentManager.RemoveComponent(componentId, worldId, entityId);
							}
						}

						entityData = default;

						worldData.FreeEntityIndices.Add(entityId);
					}

					return true;
				}
			}

			return false;
		}

		internal static Entity CloneEntity(int sourceWorldId, int sourceEntityId, int destinationWorldId)
		{
			var clone = CreateEntity(destinationWorldId, true);

			CopyEntityComponents(sourceWorldId, sourceEntityId, destinationWorldId, clone.Id);

			return clone;
		}

		internal static void CopyEntityComponents(int sourceWorldId, int sourceEntityId, int destinationWorldId, int destinationEntityId)
		{
			var entityData = worldDataById[sourceWorldId].EntityData[sourceEntityId];

			foreach (int componentId in entityData.PresentComponentTypes) {
				ComponentManager.CopyComponent(componentId, sourceWorldId, sourceEntityId, destinationWorldId, destinationEntityId);
			}
		}

		internal static bool GetEntityIsActive(in Entity entity)
			=> worldDataById[entity.WorldId].EntityData[entity.Id].IsActive;

		internal static void SetEntityIsActive(in Entity entity, bool value)
		{
			ref var worldData = ref worldDataById[entity.WorldId];
			ref bool isActive = ref worldData.EntityData[entity.Id].IsActive;

			if (value != isActive) {
				if (value) {
					worldData.InactiveEntityIds.Remove(entity.Id);
					worldData.ActiveEntityIds.Add(entity.Id);
				} else {
					worldData.ActiveEntityIds.Remove(entity.Id);
					worldData.InactiveEntityIds.Add(entity.Id);
				}

				isActive = value;

				UpdateEntitySets(entity);
			}
		}

		internal static EntityEnumerator ReadEntities(int worldId, bool? active = true)
		{
			var worldData = worldDataById[worldId];
			var entityIds = active switch {
				true => worldData.ActiveEntityIds,
				false => worldData.InactiveEntityIds,
				null => worldData.AllEntityIds
			};

			return new EntityEnumerator(worldId, entityIds);
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
			foreach (int entityId in worldData.AllEntityIds) {
				entitySet.OnEntityUpdated(new Entity(entityId, worldId));
			}

			entitySetByExpression[predicate] = entitySet;

			worldData.EntitySets.Add(entitySet);

			return entitySet;
		}

		internal static void OnEntityComponentAdded<T>(Entity entity) where T : struct
		{
			int componentId = ComponentManager.GetComponentId<T>();
			var presentComponentTypes = worldDataById[entity.WorldId].EntityData[entity.Id].PresentComponentTypes;

			lock (presentComponentTypes) {
				presentComponentTypes.Add(componentId);
			}

			UpdateEntitySets(entity);
		}

		internal static void OnEntityComponentRemoved<T>(Entity entity) where T : struct
		{
			var presentComponentTypes = worldDataById[entity.WorldId].EntityData[entity.Id].PresentComponentTypes;

			lock (presentComponentTypes) {
				presentComponentTypes.Remove(ComponentManager.GetComponentId<T>());
			}

			UpdateEntitySets(entity);
		}

		private static void OnWorldCreated(World world, WorldCreationOptions options)
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

		private static void UpdateEntitySets(in Entity entity)
		{
			var worldData = worldDataById[entity.WorldId];

			foreach (var entitySet in worldData.EntitySets) {
				entitySet.OnEntityUpdated(entity);
			}
		}
	}
}
