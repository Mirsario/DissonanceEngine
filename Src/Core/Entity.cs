namespace Dissonance.Engine
{
	public readonly struct Entity
	{
		internal readonly int Id;

		internal Entity(int id)
		{
			Id = id;
		}

		public bool HasComponent<T>() where T : struct, IComponent
			=> ComponentManager.HasComponent<T>(Id);

		public ref T GetComponent<T>() where T : struct, IComponent
			=> ref ComponentManager.GetComponent<T>(Id);

		public void SetComponent<T>(T value) where T : struct, IComponent
			=> ComponentManager.SetComponent(Id, value);
	}
}
