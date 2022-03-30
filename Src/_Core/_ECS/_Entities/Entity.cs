using System;
using Dissonance.Engine.IO;
using Newtonsoft.Json;

namespace Dissonance.Engine
{
	[JsonConverter(typeof(EntityPrefabJsonConverter))]
	public readonly struct Entity : IEntity
	{
		public readonly int Id;
		public readonly int WorldId;

		public World World => WorldManager.GetWorld(WorldId);

		public bool IsActive {
			get => EntityManager.GetEntityIsActive(this);
			set => EntityManager.SetEntityIsActive(this, value);
		}

		internal Entity(int id, int worldId)
		{
			Id = id;
			WorldId = worldId;
		}

		/// <summary>
		/// Returns whether or not this entity contains a value for the specified component type.
		/// </summary>
		/// <typeparam name="T"> The component type. </typeparam>
		public bool Has<T>() where T : struct
			=> ComponentManager.HasComponent<T>(WorldId, Id);

		/// <summary>
		/// Returns a reference to this entity's value for the specified component type. Throws exceptions on failure.
		/// </summary>
		/// <typeparam name="T"> The component type. </typeparam>
		/// <exception cref="IndexOutOfRangeException" />
		public ref T Get<T>() where T : struct
			=> ref ComponentManager.GetComponent<T>(WorldId, Id);

		public ref T Set<T>(in T value) where T : struct
			=> ref ComponentManager.SetComponent(WorldId, Id, value);

		public ref T GetOrSet<T>(Func<T> valueGetter) where T : struct
			=> ref ComponentManager.GetOrSetComponent(WorldId, Id, valueGetter);

		public void Remove<T>() where T : struct
			=> ComponentManager.RemoveComponent<T>(WorldId, Id);

		public void Destroy()
			=> EntityManager.RemoveEntity(WorldId, Id);

		/// <summary>
		/// Copies this entity's components into the provided destination entity.
		/// <br/> Components that were already present on the destination entity get fully overwritten in case of overlap, and remain untouched otherwise.
		/// </summary>
		/// <param name="destinationEntity"> The entity to copy components into. </param>
		public void CopyInto(Entity destinationEntity)
			=> EntityManager.CopyEntityComponents(WorldManager.PrefabWorldId, Id, destinationEntity.WorldId, destinationEntity.Id);

		public Entity Clone(World world)
			=> EntityManager.CloneEntity(WorldId, Id, world.Id);
	}
}
