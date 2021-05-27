namespace Dissonance.Engine
{
	public interface IEntity
	{
		public bool Has<T>() where T : struct, IComponent;

		public ref T Get<T>() where T : struct, IComponent;

		public void Set<T>(in T value) where T : struct, IComponent;
	}
}
