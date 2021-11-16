using System;
using Dissonance.Engine.IO;
using Newtonsoft.Json;

namespace Dissonance.Engine
{
	[JsonConverter(typeof(EntityPrefabJsonConverter))]
	public readonly struct EntityPrefab
	{
		internal readonly int Id;

		internal EntityPrefab(int id)
		{
			Id = id;
		}

		public override bool Equals(object obj)
			=> obj is EntityPrefab prefab && Id == prefab.Id;

		public override int GetHashCode()
			=> Id;

		public bool Has<T>() where T : struct
			=> ComponentManager.HasComponent<T>(WorldManager.PrefabWorldId, Id);

		public ref T Get<T>() where T : struct
			=> ref ComponentManager.GetComponent<T>(WorldManager.PrefabWorldId, Id);

		public Entity Clone(World world)
			=> EntityManager.CloneEntity(WorldManager.PrefabWorldId, Id, world.Id);

		internal void Set<T>(in T value) where T : struct
			=> ComponentManager.SetComponent(WorldManager.PrefabWorldId, Id, value, sendMessages: false);

		internal void Remove<T>() where T : struct
			=> ComponentManager.RemoveComponent<T>(WorldManager.PrefabWorldId, Id, sendMessages: false);

		internal void Destroy()
			=> EntityManager.RemoveEntity(WorldManager.PrefabWorldId, Id);

		public static bool operator ==(EntityPrefab left, EntityPrefab right)
			=> left.Id == right.Id;

		public static bool operator !=(EntityPrefab left, EntityPrefab right)
			=> left.Id != right.Id;
	}
}
