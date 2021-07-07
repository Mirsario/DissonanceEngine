namespace Dissonance.Engine
{
	public readonly struct Entity : IEntity
	{
		internal readonly int Id;
		internal readonly int WorldId;

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
			=> ComponentManager.SetComponent(this, value);
	}
}
