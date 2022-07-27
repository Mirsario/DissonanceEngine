using System;
using Dissonance.Engine.IO;
using Newtonsoft.Json;

namespace Dissonance.Engine;

[JsonConverter(typeof(EntityPrefabJsonConverter))]
public readonly struct EntityPrefab
{
	public readonly int Id;

	public EntityPrefab(int id)
	{
		Id = id;
	}

	public override bool Equals(object obj)
		=> obj is EntityPrefab prefab && Id == prefab.Id;

	public override int GetHashCode()
		=> Id;

	/// <summary>
	/// Returns whether or not this entity prefab contains a value for the specified component type.
	/// </summary>
	/// <typeparam name="T"> The component type. </typeparam>
	public bool Has<T>() where T : struct
		=> ComponentManager.HasComponent<T>(WorldManager.PrefabWorldId, Id);

	/// <summary>
	/// Returns a reference to this entity prefab's value for the specified component type. Throws exceptions on failure.
	/// </summary>
	/// <typeparam name="T"> The component type. </typeparam>
	/// <exception cref="IndexOutOfRangeException" />
	public ref T Get<T>() where T : struct
		=> ref ComponentManager.GetComponent<T>(WorldManager.PrefabWorldId, Id);

	/// <summary>
	/// Creates an entity in the provided world and copies this prefab's components into it, effectively instantiating the prefab.
	/// </summary>
	/// <param name="world"> The world to create a clone in. </param>
	/// <returns> The clone entity. </returns>
	public Entity Clone(World world)
		=> EntityManager.CloneEntity(WorldManager.PrefabWorldId, Id, world.Id);

	/// <summary>
	/// Copies this entity prefab's components into the provided destination entity.
	/// <br/> Components that were already present on the destination entity get fully overwritten in case of overlap, and remain untouched otherwise.
	/// </summary>
	/// <param name="destinationEntity"> The entity to copy components into. </param>
	public void CopyInto(Entity destinationEntity)
		=> EntityManager.CopyEntityComponents(WorldManager.PrefabWorldId, Id, destinationEntity.WorldId, destinationEntity.Id);

	internal ref T Set<T>(in T value) where T : struct
		=> ref ComponentManager.SetComponent(WorldManager.PrefabWorldId, Id, value, sendMessages: false);

	internal ref T GetOrSet<T>(Func<T> valueGetter) where T : struct
		=> ref ComponentManager.GetOrSetComponent(WorldManager.PrefabWorldId, Id, valueGetter, sendMessages: false);

	internal void Remove<T>() where T : struct
		=> ComponentManager.RemoveComponent<T>(WorldManager.PrefabWorldId, Id, sendMessages: false);

	internal void Destroy()
		=> EntityManager.RemoveEntity(WorldManager.PrefabWorldId, Id);

	public static bool operator ==(EntityPrefab left, EntityPrefab right)
		=> left.Id == right.Id;

	public static bool operator !=(EntityPrefab left, EntityPrefab right)
		=> left.Id != right.Id;

	public static explicit operator Entity(EntityPrefab prefab) => new(prefab.Id, WorldManager.PrefabWorldId);
}
