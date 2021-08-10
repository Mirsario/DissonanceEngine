using System;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public sealed class ComponentManager : EngineModule
	{
		private static class ComponentData<T> where T : struct
		{
			public struct WorldData
			{
				public static WorldData Default = new() {
					Data = Array.Empty<T>(),
					IndicesByEntity = Array.Empty<int>(),
					GlobalDataIndex = -1
				};

				public T[] Data;
				public int[] IndicesByEntity;
				public int GlobalDataIndex;
			}

			public static T GlobalSingleton;
			public static WorldData[] DataByWorld = Array.Empty<WorldData>();
		}

		internal static Action<Entity, Type> OnComponentAdded;
		internal static Action<Entity, Type> OnComponentRemoved;

		internal static bool HasComponent<T>(int worldId) where T : struct
		{
			if(worldId >= ComponentData<T>.DataByWorld.Length) {
				return false;
			}

			return ComponentData<T>.DataByWorld[worldId].GlobalDataIndex >= 0;
		}

		internal static bool HasComponent<T>(int worldId, int entityId) where T : struct
		{
			if(worldId >= ComponentData<T>.DataByWorld.Length) {
				return false;
			}

			int[] indicesByEntity = ComponentData<T>.DataByWorld[worldId].IndicesByEntity;

			if(entityId >= indicesByEntity.Length) {
				return false;
			}

			return indicesByEntity[entityId] != -1;
		}

		internal static ref T GetComponent<T>() where T : struct
			=> ref ComponentData<T>.GlobalSingleton;

		internal static ref T GetComponent<T>(int worldId) where T : struct
		{
			ref var worldData = ref ComponentData<T>.DataByWorld[worldId];

			return ref worldData.Data[worldData.GlobalDataIndex];
		}

		internal static ref T GetComponent<T>(int worldId, int entityId) where T : struct
		{
			ref var worldData = ref ComponentData<T>.DataByWorld[worldId];

			return ref worldData.Data[worldData.IndicesByEntity[entityId]];
		}

		internal static void SetComponent<T>(T value) where T : struct
			=> ComponentData<T>.GlobalSingleton = value;

		internal static void SetComponent<T>(int worldId, T value) where T : struct
		{
			if(ComponentData<T>.DataByWorld.Length <= worldId) {
				ArrayUtils.ResizeAndFillArray(ref ComponentData<T>.DataByWorld, worldId + 1, ComponentData<T>.WorldData.Default);
			}

			ref var worldData = ref ComponentData<T>.DataByWorld[worldId];
			ref int dataId = ref worldData.GlobalDataIndex;

			if(dataId <= 0) {
				dataId = worldData.Data.Length;

				Array.Resize(ref worldData.Data, dataId + 1);
			}

			worldData.Data[dataId] = value;
		}

		internal static void SetComponent<T>(in Entity entity, T value) where T : struct
		{
			if(ComponentData<T>.DataByWorld.Length <= entity.WorldId) {
				ArrayUtils.ResizeAndFillArray(ref ComponentData<T>.DataByWorld, entity.WorldId + 1, ComponentData<T>.WorldData.Default);
			}

			ref var worldData = ref ComponentData<T>.DataByWorld[entity.WorldId];

			int dataId = -1;

			if(entity.Id >= worldData.IndicesByEntity.Length) {
				ArrayUtils.ResizeAndFillArray(ref worldData.IndicesByEntity, entity.Id + 1, -1);
			} else {
				dataId = worldData.IndicesByEntity[entity.Id];
			}

			if(dataId < 0) {
				dataId = worldData.Data.Length;
				worldData.IndicesByEntity[entity.Id] = dataId;

				Array.Resize(ref worldData.Data, dataId + 1);

				worldData.Data[dataId] = value;

				OnComponentAdded?.Invoke(entity, typeof(T));
			} else {
				worldData.Data[dataId] = value;
			}
		}

		internal static void RemoveComponent<T>(in Entity entity) where T : struct
		{
			if(entity.WorldId < 0 || entity.WorldId >= ComponentData<T>.DataByWorld.Length) {
				return;
			}

			ref var worldData = ref ComponentData<T>.DataByWorld[entity.WorldId];

			if(entity.Id < 0 || entity.Id >= worldData.IndicesByEntity.Length) {
				return;
			}

			int dataIndex = worldData.IndicesByEntity[entity.Id];

			if(dataIndex < 0) {
				return;
			}

			worldData.IndicesByEntity[entity.Id] = -1;

			OnComponentRemoved?.Invoke(entity, typeof(T));
		}
	}
}
