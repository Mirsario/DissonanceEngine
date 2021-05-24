namespace Dissonance.Engine
{
	public interface IEntity
	{
		public bool HasComponent<T>() where T : struct, IComponent;

		public ref T GetComponent<T>() where T : struct, IComponent;

		public void SetComponent<T>(T value) where T : struct, IComponent;
	}
}
