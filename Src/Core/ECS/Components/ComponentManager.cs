using System;
using System.Collections.Generic;
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
			public static bool HasGlobalSingleton;
			public static WorldData[] DataByWorld = Array.Empty<WorldData>();
		}

		internal static Action<Entity, Type> OnComponentAdded;
		internal static Action<Entity, Type> OnComponentRemoved;

		private static Dictionary<string, Type> structureTypesByName = new();

		protected override void PreInit()
		{
			// By-name type lookups are used in prefab parsing.
			AssemblyRegistrationModule.OnAssemblyRegistered += (assembly, types) => {
				foreach (var type in types) {
					if (!type.IsValueType || type.IsAbstract || type.IsNested) {
						continue;
					}

					if (!structureTypesByName.TryGetValue(type.Name, out var existingType)) {
						structureTypesByName[type.Name] = type;
					} else {
						//TODO: Use minimal unique paths.
						structureTypesByName[type.Name] = null;
						structureTypesByName[type.FullName] = type;
						structureTypesByName[existingType.FullName] = existingType;
					}
				}
			};
		}

		public static Type GetComponentTypeFromName(string name)
		{
			if (structureTypesByName.TryGetValue(name, out var type)) {
				if (type == null) {
					throw new ArgumentException($"Component name '{name}' is ambiguous.");
				}

				return type;
			}

			throw new KeyNotFoundException($"Couldn't find component with the provided name '{name}'.");
		}

		public static bool TryGetComponentTypeFromName(string name, out Type type)
		{
			return structureTypesByName.TryGetValue(name, out type) && type != null;
		}

		internal static bool HasComponent<T>() where T : struct
		{
			return ComponentData<T>.HasGlobalSingleton;
		}

		internal static bool HasComponent<T>(int worldId, int entityId) where T : struct
		{
			if (worldId >= ComponentData<T>.DataByWorld.Length) {
				return false;
			}

			int[] indicesByEntity = ComponentData<T>.DataByWorld[worldId].IndicesByEntity;

			if (entityId >= indicesByEntity.Length) {
				return false;
			}

			return indicesByEntity[entityId] != -1;
		}

		internal static ref T GetComponent<T>() where T : struct
		{
			if (!HasComponent<T>()) {
				throw new InvalidOperationException($"No global value of component '{typeof(T).Name}' set.");
			}

			return ref ComponentData<T>.GlobalSingleton;
		}

		internal static ref T GetComponent<T>(int worldId, int entityId) where T : struct
		{
			ref var worldData = ref ComponentData<T>.DataByWorld[worldId];

			return ref worldData.Data[worldData.IndicesByEntity[entityId]];
		}

		internal static void SetComponent<T>(T value) where T : struct
		{
			ComponentData<T>.GlobalSingleton = value;
			ComponentData<T>.HasGlobalSingleton = true;
		}

		internal static void SetComponent<T>(in Entity entity, T value) where T : struct
		{
			if (ComponentData<T>.DataByWorld.Length <= entity.WorldId) {
				ArrayUtils.ResizeAndFillArray(ref ComponentData<T>.DataByWorld, entity.WorldId + 1, ComponentData<T>.WorldData.Default);
			}

			ref var worldData = ref ComponentData<T>.DataByWorld[entity.WorldId];

			int dataId = -1;

			if (entity.Id >= worldData.IndicesByEntity.Length) {
				ArrayUtils.ResizeAndFillArray(ref worldData.IndicesByEntity, entity.Id + 1, -1);
			} else {
				dataId = worldData.IndicesByEntity[entity.Id];
			}

			if (dataId < 0) {
				dataId = worldData.Data.Length;
				worldData.IndicesByEntity[entity.Id] = dataId;

				Array.Resize(ref worldData.Data, dataId + 1);

				worldData.Data[dataId] = value;

				OnComponentAdded?.Invoke(entity, typeof(T));
				MessageManager.SendMessage(entity.WorldId, new ComponentAddedMessage<T>(entity, value));
			} else {
				worldData.Data[dataId] = value;
			}
		}

		internal static void RemoveComponent<T>(in Entity entity) where T : struct
		{
			if (entity.WorldId < 0 || entity.WorldId >= ComponentData<T>.DataByWorld.Length) {
				return;
			}

			ref var worldData = ref ComponentData<T>.DataByWorld[entity.WorldId];

			if (entity.Id < 0 || entity.Id >= worldData.IndicesByEntity.Length) {
				return;
			}

			int dataIndex = worldData.IndicesByEntity[entity.Id];

			if (dataIndex < 0) {
				return;
			}

			var value = worldData.Data[dataIndex];

			worldData.IndicesByEntity[entity.Id] = -1;

			OnComponentRemoved?.Invoke(entity, typeof(T));
			MessageManager.SendMessage(entity.WorldId, new ComponentRemovedMessage<T>(entity, value));
		}
	}
}
