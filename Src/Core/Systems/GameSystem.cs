using System;

namespace Dissonance.Engine
{
	public abstract class GameSystem
	{
		public World World { get; internal set; }

		protected internal SystemTypeData TypeData { get; }
		internal bool Initialized { get; set; }

		protected GameSystem()
		{
			TypeData = SystemManager.SystemTypeInfo[GetType()];
		}

		protected internal virtual void Initialize() { }
		protected internal virtual void FixedUpdate() { }
		protected internal virtual void RenderUpdate() { }

		protected ref T GlobalGet<T>() where T : struct
			=> ref ComponentManager.GetComponent<T>();

		protected void GlobalSet<T>(T value) where T : struct
			=> ComponentManager.SetComponent(value);

		protected bool WorldHas<T>() where T : struct
			=> World.Has<T>();

		protected ref T WorldGet<T>() where T : struct
			=> ref World.Get<T>();

		protected void WorldSet<T>(T value) where T : struct
			=> World.Set(value);

		protected ReadOnlySpan<Entity> ReadEntities()
			=> World.ReadEntities();

		protected void SendMessage<T>(in T message) where T : struct
			=> World.SendMessage(message);

		protected ReadOnlySpan<T> ReadMessages<T>() where T : struct
			=> World.ReadMessages<T>();
	}
}
