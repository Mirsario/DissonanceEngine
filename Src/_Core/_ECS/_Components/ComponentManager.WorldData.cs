using System;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine;

unsafe partial class ComponentManager
{
	partial class ComponentData<T>
	{
		public struct ComponentWorldData
		{
			public T WorldComponent;
			public bool HasWorldComponent;
		}
		
		public static ComponentWorldData[] WorldData = Array.Empty<ComponentWorldData>();

		public static ComponentWorldFunctions GetComponentWorldFunctions() => new() {
			Has = &Has,
			Remove = &Remove,
			Copy = &Copy,
		};

		public static bool Has(int worldId)
			=> HasComponent<T>(worldId);

		public static void Remove(int worldId)
			=> RemoveComponent<T>(worldId);
		
		public static void Copy(int sourceWorldId, int destinationWorldId)
			=> SetComponent(destinationWorldId, GetComponent<T>(sourceWorldId));
	}

	private readonly struct ComponentWorldFunctions
	{
		public delegate*<int, bool> Has { get; init; }
		public delegate*<int, void> Remove { get; init; }
		public delegate*<int, int, void> Copy { get; init; }
	}
	
	// By component id

	internal static bool HasComponent(int componentTypeId, int worldId)
		=> ComponentTypeDataById[componentTypeId].WorldFunctions.Has(worldId);

	internal static void RemoveComponent(int componentTypeId, int worldId)
		=> ComponentTypeDataById[componentTypeId].WorldFunctions.Remove(worldId);

	internal static void CopyComponent(int componentTypeId, int sourceWorldId, int destinationWorldId)
		=> ComponentTypeDataById[componentTypeId].WorldFunctions.Copy(sourceWorldId, destinationWorldId);

	// By type parameters

	internal static bool HasComponent<T>(int worldId) where T : struct
	{
		if (worldId >= ComponentData<T>.WorldData.Length) {
			return false;
		}

		return ComponentData<T>.WorldData[worldId].HasWorldComponent;
	}

	internal static ref T GetComponent<T>(int worldId) where T : struct
	{
		ref var worldData = ref ComponentData<T>.WorldData[worldId];

		if (!worldData.HasWorldComponent) {
			throw new InvalidOperationException($"Tried to get component of type {typeof(T)} from world {worldId}, but it was not present.");
		}

		return ref worldData.WorldComponent;
	}

	internal static ref T GetOrSetComponent<T>(int worldId, Func<T> valueGetter) where T : struct
	{
		if (!HasComponent<T>(worldId)) {
			return ref SetComponent(worldId, valueGetter());
		}

		return ref GetComponent<T>(worldId);
	}

	internal static ref T SetComponent<T>(int worldId, T value) where T : struct
	{
		if (ComponentData<T>.WorldData.Length <= worldId) {
			ArrayUtils.ResizeAndFillArray(ref ComponentData<T>.WorldData, worldId + 1, new());
		}

		ref var worldData = ref ComponentData<T>.WorldData[worldId];

		worldData.WorldComponent = value;
		worldData.HasWorldComponent = true;

		return ref worldData.WorldComponent;
	}

	internal static void RemoveComponent<T>(int worldId) where T : struct
	{
		if (worldId < 0 || worldId >= ComponentData<T>.WorldData.Length) {
			return;
		}

		ref var worldData = ref ComponentData<T>.WorldData[worldId];

		worldData.WorldComponent = default;
		worldData.HasWorldComponent = false;
	}
}
