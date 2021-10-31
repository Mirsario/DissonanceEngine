namespace Dissonance.Engine
{
	public readonly struct Entity : IEntity
	{
		internal readonly int Id;
		internal readonly int WorldId;

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

		public bool Has<T>() where T : struct
			=> ComponentManager.HasComponent<T>(WorldId, Id);

		public ref T Get<T>() where T : struct
			=> ref ComponentManager.GetComponent<T>(WorldId, Id);

		public void Set<T>(in T value) where T : struct
			=> ComponentManager.SetComponent(WorldId, Id, value);

		public void Remove<T>() where T : struct
			=> ComponentManager.RemoveComponent<T>(WorldId, Id);

		public void Destroy()
			=> EntityManager.RemoveEntity(WorldId, Id);

		public Entity Clone(World world)
			=> EntityManager.CloneEntity(WorldId, Id, world.Id);
	}
}
