namespace Dissonance.Engine;

/// <summary>
/// A static interface for accessing global game data -- that is, data that isn't tied to a single world or entity.
/// </summary>
public static class Global
{
	/// <summary> Returns whether or not the global context contains a <typeparamref name="T"/> component. </summary>
	/// <typeparam name="T"> The component type. </typeparam>
	public static bool Has<T>() where T : struct
		=> ComponentManager.HasComponent<T>();

	/// <summary> Attempts to get and return a reference to the global context's <typeparamref name="T"/> component. </summary>
	/// <typeparam name="T"> The component type. </typeparam>
	public static ref T Get<T>() where T : struct
		=> ref ComponentManager.GetComponent<T>();

	/// <summary> Sets a value for the global context's <typeparamref name="T"/> component. </summary>
	/// <param name="value"> The provided component value. </param>
	/// <typeparam name="T"> The component type. </typeparam>
	public static void Set<T>(T value) where T : struct
		=> ComponentManager.SetComponent(value);

	/// <summary> Removes global context's <typeparamref name="T"/> component, if present. </summary>
	/// <typeparam name="T"> The component type. </typeparam>
	public static void Remove<T>() where T : struct
		=> ComponentManager.RemoveComponent<T>();
}
