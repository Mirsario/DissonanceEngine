using System;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public sealed class ComponentManager : EngineModule
	{
		private static class ComponentData<T> where T : struct, IComponent
		{
			public struct WorldData
			{
				public static WorldData Default = new() {
					data = Array.Empty<T>(),
					indicesByEntity = Array.Empty<int>(),
					globalDataIndex = -1
				};

				public T[] data;
				public int[] indicesByEntity;
				public int globalDataIndex;
			}

			public static WorldData[] worldData = Array.Empty<WorldData>();
		}

		internal static bool HasComponent<T>(int worldId) where T : struct, IComponent
		{
			if(worldId >= ComponentData<T>.worldData.Length) {
				return false;
			}

			return ComponentData<T>.worldData[worldId].globalDataIndex >= 0;
		}

		internal static bool HasComponent<T>(int worldId, int entityId) where T : struct, IComponent
		{
			if(worldId >= ComponentData<T>.worldData.Length) {
				return false;
			}

			int[] indicesByEntity = ComponentData<T>.worldData[worldId].indicesByEntity;

			if(entityId >= indicesByEntity.Length) {
				return false;
			}

			return indicesByEntity[entityId] != -1;
		}

		internal static ref T GetComponent<T>(int worldId) where T : struct, IComponent
		{
			ref var worldData = ref ComponentData<T>.worldData[worldId];

			return ref worldData.data[worldData.globalDataIndex];
		}

		internal static ref T GetComponent<T>(int worldId, int entityId) where T : struct, IComponent
		{
			ref var worldData = ref ComponentData<T>.worldData[worldId];

			return ref worldData.data[worldData.indicesByEntity[entityId]];
		}

		internal static void SetComponent<T>(int worldId, T value) where T : struct, IComponent
		{
			if(ComponentData<T>.worldData.Length <= worldId) {
				ArrayUtils.ResizeAndFillArray(ref ComponentData<T>.worldData, worldId + 1, ComponentData<T>.WorldData.Default);
			}

			ref var worldData = ref ComponentData<T>.worldData[worldId];
			ref int dataId = ref worldData.globalDataIndex;

			if(dataId <= 0) {
				dataId = worldData.data.Length;

				Array.Resize(ref worldData.data, dataId + 1);
			}

			worldData.data[dataId] = value;
		}

		internal static void SetComponent<T>(in Entity entity, T value) where T : struct, IComponent
		{
			if(ComponentData<T>.worldData.Length <= entity.WorldId) {
				ArrayUtils.ResizeAndFillArray(ref ComponentData<T>.worldData, entity.WorldId + 1, ComponentData<T>.WorldData.Default);
			}

			ref var worldData = ref ComponentData<T>.worldData[entity.WorldId];

			int dataId = -1;

			if(entity.Id >= worldData.indicesByEntity.Length) {
				ArrayUtils.ResizeAndFillArray(ref worldData.indicesByEntity, entity.Id + 1, -1);
			} else {
				dataId = worldData.indicesByEntity[entity.Id];
			}
			
			if(dataId < 0) {
				dataId = worldData.data.Length;
				worldData.indicesByEntity[entity.Id] = dataId;
				Array.Resize(ref worldData.data, dataId + 1);
			}

			worldData.data[dataId] = value;

			WorldManager.GetWorld(entity.WorldId).OnEntityUpdated(entity); //Too slow?
		}
	}
}
