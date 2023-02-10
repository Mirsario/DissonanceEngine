using System;
using System.Collections.Concurrent;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine;

unsafe partial class ComponentManager
{
	partial class ComponentData<T>
	{
		public struct ComponentWorldEntityData
		{
			public readonly object DataResizingLock = new();
			public readonly object IndicesByEntityResizingLock = new();

			public T[] Data = Array.Empty<T>();
			public int[] IndicesByEntity = Array.Empty<int>();
			public int NextDataIndex = default;
			public ConcurrentBag<int> FreeDataIndices = new();

			public ComponentWorldEntityData() { }
		}

		public static ComponentWorldEntityData[] WorldEntityData = Array.Empty<ComponentWorldEntityData>();

		public static ComponentEntityFunctions GetComponentEntityFunctions() => new() {
			Has = &Has,
			Remove = &Remove,
			Copy = &Copy,
		};

		public static bool Has(int worldId, int entityId)
			=> HasComponent<T>(worldId, entityId);

		public static void Remove(int worldId, int entityId)
			=> RemoveComponent<T>(worldId, entityId);
		
		public static void Copy(int sourceWorldId, int sourceEntityId, int destinationWorldId, int destinationEntityId)
			=> SetComponent(destinationWorldId, destinationEntityId, GetComponent<T>(sourceWorldId, sourceEntityId));
	}

	private readonly struct ComponentEntityFunctions
	{
		public delegate*<int, int, bool> Has { get; init; }
		public delegate*<int, int, void> Remove { get; init; }
		public delegate*<int, int, int, int, void> Copy { get; init; }
	}
	
	// By component id

	internal static bool HasComponent(int componentTypeId, int worldId, int entityId)
		=> ComponentTypeDataById[componentTypeId].EntityFunctions.Has(worldId, entityId);

	internal static void RemoveComponent(int componentTypeId, int worldId, int entityId)
		=> ComponentTypeDataById[componentTypeId].EntityFunctions.Remove(worldId, entityId);

	internal static void CopyComponent(int componentTypeId, int sourceWorldId, int sourceEntityId, int destinationWorldId, int destinationEntityId)
		=> ComponentTypeDataById[componentTypeId].EntityFunctions.Copy(sourceWorldId, sourceEntityId, destinationWorldId, destinationEntityId);

	// By type parameters

	internal static bool HasComponent<T>(int worldId, int entityId) where T : struct
	{
		if (worldId >= ComponentData<T>.WorldEntityData.Length) {
			return false;
		}

		int[] indicesByEntity = ComponentData<T>.WorldEntityData[worldId].IndicesByEntity;

		if (entityId >= indicesByEntity.Length) {
			return false;
		}

		return indicesByEntity[entityId] != -1;
	}

	internal static ref T GetComponent<T>(int worldId, int entityId) where T : struct
	{
		ref var worldData = ref ComponentData<T>.WorldEntityData[worldId];

		return ref worldData.Data[worldData.IndicesByEntity[entityId]];
	}

	internal static ref T GetOrSetComponent<T>(int worldId, int entityId, Func<T> valueGetter, bool sendMessages = true) where T : struct
	{
		if (!HasComponent<T>(worldId, entityId)) {
			return ref SetComponent(worldId, entityId, valueGetter(), sendMessages);
		}

		return ref GetComponent<T>(worldId, entityId);
	}

	internal static ref T SetComponent<T>(int worldId, int entityId, T value, bool sendMessages = true) where T : struct
	{
		if (ComponentData<T>.WorldEntityData.Length <= worldId) {
			ArrayUtils.ResizeAndFillArray(ref ComponentData<T>.WorldEntityData, worldId + 1, new());
		}

		ref var worldData = ref ComponentData<T>.WorldEntityData[worldId];

		int dataId = -1;

		if (entityId >= worldData.IndicesByEntity.Length) {
			lock (worldData.IndicesByEntityResizingLock) {
				int newSize = Math.Max(1, worldData.IndicesByEntity.Length);

				while (newSize <= entityId) {
					newSize *= 2;
				}

				ArrayUtils.ResizeAndFillArray(ref worldData.IndicesByEntity, newSize, -1);
			}
		} else {
			dataId = worldData.IndicesByEntity[entityId];
		}

		if (dataId < 0) {
			if (!worldData.FreeDataIndices.TryTake(out dataId)) {
				lock (worldData.DataResizingLock) {
					dataId = worldData.NextDataIndex++;

					if (dataId >= worldData.Data.Length) {
						int newSize = Math.Max(1, worldData.Data.Length);

						while (newSize <= dataId) {
							newSize *= 2;
						}

						Array.Resize(ref worldData.Data, newSize);
					}
				}
			}

			worldData.IndicesByEntity[entityId] = dataId;
			worldData.Data[dataId] = value;

			var entity = new Entity(entityId, worldId);

			EntityManager.OnEntityComponentAdded<T>(entity);

			if (sendMessages) {
				MessageManager.SendMessage(worldId, new ComponentAddedMessage<T>(entity, value));
			}
		} else {
			worldData.Data[dataId] = value;
		}

		return ref worldData.Data[dataId];
	}

	internal static void RemoveComponent<T>(int worldId, int entityId, bool sendMessages = true) where T : struct
	{
		if (worldId < 0 || worldId >= ComponentData<T>.WorldEntityData.Length) {
			return;
		}

		ref var worldData = ref ComponentData<T>.WorldEntityData[worldId];

		if (entityId < 0 || entityId >= worldData.IndicesByEntity.Length) {
			return;
		}

		int dataIndex = worldData.IndicesByEntity[entityId];

		if (dataIndex < 0) {
			return;
		}

		var value = worldData.Data[dataIndex];

		worldData.IndicesByEntity[entityId] = -1;

		var entity = new Entity(entityId, worldId);

		EntityManager.OnEntityComponentRemoved<T>(entity);
		MessageManager.SendMessage(worldId, new ComponentRemovedMessage<T>(entity, value));
	}
}
