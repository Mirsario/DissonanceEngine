using System;

namespace Dissonance.Engine
{
	public abstract class GameSystem
	{
		public World World { get; internal set; }

		internal SystemTypeInfo TypeData { get; }
		internal bool Initialized { get; set; }

		protected GameSystem()
		{
			TypeData = SystemManager.SystemTypeInfo[GetType()];
		}

		public virtual void Initialize() { }
		public virtual void FixedUpdate() { }
		public virtual void RenderUpdate() { }

		public ref T GlobalGet<T>() where T : struct
			=> ref ComponentManager.GetComponent<T>();

		public void GlobalSet<T>(T value) where T : struct
			=> ComponentManager.SetComponent(value);

		public bool WorldHas<T>() where T : struct
			=> World.Has<T>();

		public ref T WorldGet<T>() where T : struct
			=> ref World.Get<T>();

		public void WorldSet<T>(T value) where T : struct
			=> World.Set(value);

		public ReadOnlySpan<Entity> ReadEntities()
			=> World.ReadEntities();

		public void SendMessage<T>(in T message) where T : struct, IMessage
			=> World.SendMessage(message);

		public ReadOnlySpan<T> ReadMessages<T>() where T : struct, IMessage
			=> World.ReadMessages<T>();
	}
}
