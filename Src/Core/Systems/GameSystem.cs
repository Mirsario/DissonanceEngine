using System;

namespace Dissonance.Engine
{
	public abstract class GameSystem
	{
		public World World { get; internal set; }

		public virtual void Initialize() { }
		public virtual void FixedUpdate() { }
		public virtual void RenderUpdate() { }

		public bool Has<T>() where T : struct, IComponent
			=> World.Has<T>();

		public ref T Get<T>() where T : struct, IComponent
			=> ref World.Get<T>();

		public void Set<T>(T value) where T : struct, IComponent
			=> World.Set(value);

		public ReadOnlySpan<Entity> ReadEntities()
			=> World.ReadEntities();
	}
}
