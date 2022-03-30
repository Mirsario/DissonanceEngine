using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public unsafe sealed class ComponentManager : EngineModule
	{
		private static class ComponentData<T> where T : struct
		{
			public struct ComponentWorldData
			{
				public readonly object DataResizingLock = new();
				public readonly object IndicesByEntityResizingLock = new();

				public T[] Data = Array.Empty<T>();
				public int[] IndicesByEntity = Array.Empty<int>();
				public int GlobalDataIndex = -1;
				public int NextDataIndex = default;
				public ConcurrentBag<int> FreeDataIndices = new();

				public ComponentWorldData() { }
			}

			public static T GlobalSingleton;
			public static bool HasGlobalSingleton;
			public static ComponentWorldData[] DataByWorld = Array.Empty<ComponentWorldData>();
			public static int TypeId;

			static ComponentData()
			{
				TypeId = ComponentTypeDataById.Count;

				var typeData = new ComponentTypeData(
					typeof(T),
					&Remove,
					&Copy
				);

				ComponentTypeDataById.Add(typeData);
			}

			public static void Remove(int worldId, int entityId)
			{
				RemoveComponent<T>(worldId, entityId);
			}

			public static void Copy(int sourceWorldId, int sourceEntityId, int destinationWorldId, int destinationEntityId)
			{
				SetComponent(destinationWorldId, destinationEntityId, GetComponent<T>(sourceWorldId, sourceEntityId));
			}
		}

		private readonly struct ComponentTypeData
		{
			public readonly Type Type;
			public readonly delegate*<int, int, void> Remove;
			public readonly delegate*<int, int, int, int, void> Copy;

			public ComponentTypeData(Type type, delegate*<int, int, void> remove, delegate*<int, int, int, int, void> copy)
			{
				Type = type;
				Remove = remove;
				Copy = copy;
			}

			public override string ToString() => Type.ToString();
		}

		private static readonly List<ComponentTypeData> ComponentTypeDataById = new();
		private static readonly Dictionary<string, Type> StructureTypesByName = new();

		protected override void InitializeForAssembly(Assembly assembly)
		{
			// By-name type lookups are used in prefab parsing.
			foreach (var type in assembly.GetTypes()) {
				if (!type.IsValueType || type.IsAbstract || type.IsNested || type.IsByRefLike || type.IsGenericTypeDefinition) {
					continue;
				}

				if (!StructureTypesByName.TryGetValue(type.Name, out var existingType)) {
					StructureTypesByName[type.Name] = type;
				} else {
					//TODO: Use minimal unique paths.
					StructureTypesByName[type.Name] = null;
					StructureTypesByName[type.FullName] = type;
					StructureTypesByName[existingType.FullName] = existingType;
				}
			}
		}

		public static Type GetComponentTypeFromName(string name)
		{
			if (StructureTypesByName.TryGetValue(name, out var type)) {
				if (type == null) {
					throw new ArgumentException($"Component name '{name}' is ambiguous.");
				}

				return type;
			}

			throw new KeyNotFoundException($"Couldn't find component with the provided name '{name}'.");
		}

		public static bool TryGetComponentTypeFromName(string name, out Type type)
		{
			return StructureTypesByName.TryGetValue(name, out type) && type != null;
		}

		internal static int GetComponentId<T>() where T : struct
			=> ComponentData<T>.TypeId;

		internal static void CopyComponent(int componentTypeId, int sourceWorldId, int sourceEntityId, int destinationWorldId, int destinationEntityId)
			=> ComponentTypeDataById[componentTypeId].Copy(sourceWorldId, sourceEntityId, destinationWorldId, destinationEntityId);

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

		internal static ref T GetOrSetComponent<T>(Func<T> valueGetter) where T : struct
		{
			if (!ComponentData<T>.HasGlobalSingleton) {
				ComponentData<T>.GlobalSingleton = valueGetter();
				ComponentData<T>.HasGlobalSingleton = true;
			}

			return ref ComponentData<T>.GlobalSingleton;
		}

		internal static ref T GetOrSetComponent<T>(int worldId, int entityId, Func<T> valueGetter, bool sendMessages = true) where T : struct
		{
			if (!HasComponent<T>(worldId, entityId)) {
				return ref SetComponent(worldId, entityId, valueGetter(), sendMessages);
			}

			return ref GetComponent<T>(worldId, entityId);
		}

		internal static ref T SetComponent<T>(T value) where T : struct
		{
			ComponentData<T>.GlobalSingleton = value;
			ComponentData<T>.HasGlobalSingleton = true;

			return ref ComponentData<T>.GlobalSingleton;
		}

		internal static ref T SetComponent<T>(int worldId, int entityId, T value, bool sendMessages = true) where T : struct
		{
			if (ComponentData<T>.DataByWorld.Length <= worldId) {
				ArrayUtils.ResizeAndFillArray(ref ComponentData<T>.DataByWorld, worldId + 1, new());
			}

			ref var worldData = ref ComponentData<T>.DataByWorld[worldId];

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

		internal static void RemoveComponent(int componentTypeId, int worldId, int entityId)
			=> ComponentTypeDataById[componentTypeId].Remove(worldId, entityId);

		internal static void RemoveComponent<T>(int worldId, int entityId, bool sendMessages = true) where T : struct
		{
			if (worldId < 0 || worldId >= ComponentData<T>.DataByWorld.Length) {
				return;
			}

			ref var worldData = ref ComponentData<T>.DataByWorld[worldId];

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
}
