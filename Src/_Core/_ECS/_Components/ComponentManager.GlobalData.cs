using System;

namespace Dissonance.Engine;

unsafe partial class ComponentManager
{
	partial class ComponentData<T>
	{
		public static T GlobalComponent;
		public static bool HasGlobalComponent;

		public static ComponentGlobalFunctions GetComponentGlobalFunctions() => new() {
			Has = &Has,
			Remove = &Remove,
		};

		public static bool Has()
			=> HasComponent<T>();

		public static void Remove()
			=> RemoveComponent<T>();
	}

	private readonly struct ComponentGlobalFunctions
	{
		public delegate*<bool> Has { get; init; }
		public delegate*<void> Remove { get; init; }
	}

	// By component id

	internal static bool HasComponent(int componentTypeId)
		=> ComponentTypeDataById[componentTypeId].GlobalFunctions.Has();

	internal static void RemoveComponent(int componentTypeId)
		=> ComponentTypeDataById[componentTypeId].GlobalFunctions.Remove();

	// By type parameters

	internal static bool HasComponent<T>() where T : struct
		=> ComponentData<T>.HasGlobalComponent;

	internal static ref T GetComponent<T>() where T : struct
	{
		if (!HasComponent<T>()) {
			throw new InvalidOperationException($"No global value of component '{typeof(T).Name}' set.");
		}

		return ref ComponentData<T>.GlobalComponent;
	}

	internal static ref T GetOrSetComponent<T>(Func<T> valueGetter) where T : struct
	{
		if (!ComponentData<T>.HasGlobalComponent) {
			ComponentData<T>.GlobalComponent = valueGetter();
			ComponentData<T>.HasGlobalComponent = true;
		}

		return ref ComponentData<T>.GlobalComponent;
	}

	internal static ref T SetComponent<T>(T value) where T : struct
	{
		ComponentData<T>.GlobalComponent = value;
		ComponentData<T>.HasGlobalComponent = true;

		return ref ComponentData<T>.GlobalComponent;
	}

	internal static void RemoveComponent<T>() where T : struct
	{
		ComponentData<T>.GlobalComponent = default;
		ComponentData<T>.HasGlobalComponent = false;
	}
}
