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

		public bool Has<T>() where T : struct, IComponent
			=> ComponentManager.HasComponent<T>(WorldId, Id);

		public ref T Get<T>() where T : struct, IComponent
			=> ref ComponentManager.GetComponent<T>(WorldId, Id);

		public void Set<T>(in T value) where T : struct, IComponent
			=> ComponentManager.SetComponent(this, value);
	}
}
