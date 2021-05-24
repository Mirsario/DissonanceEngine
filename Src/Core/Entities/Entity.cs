using System;

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

		public bool HasComponent<T>() where T : struct, IComponent
			=> ComponentManager.HasComponent<T>(WorldId, Id);

		public ref T GetComponent<T>() where T : struct, IComponent
			=> ref ComponentManager.GetComponent<T>(WorldId, Id);

		public void SetComponent<T>(T value) where T : struct, IComponent
			=> ComponentManager.SetComponent(WorldId, Id, value);
	}
}
