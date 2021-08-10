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
		{
			if(!TypeData.ReadTypes.Contains(typeof(T))) {
				throw new InvalidOperationException($"System {GetType().Name} tried to read an undeclared global component - '{typeof(T).Name}'.");
			}

			return ref ComponentManager.GetComponent<T>();
		}

		protected void GlobalSet<T>(T value) where T : struct
		{
			if(!TypeData.WriteTypes.Contains(typeof(T))) {
				throw new InvalidOperationException($"System {GetType().Name} tried to write an undeclared global component - '{typeof(T).Name}'.");
			}

			ComponentManager.SetComponent(value);
		}

		protected bool WorldHas<T>() where T : struct
			=> World.Has<T>();

		protected ref T WorldGet<T>() where T : struct
		{
			if(!TypeData.ReadTypes.Contains(typeof(T))) {
				throw new InvalidOperationException($"System {GetType().Name} tried to read an undeclared world component - '{typeof(T).Name}'.");
			}

			return ref World.Get<T>();
		}

		protected void WorldSet<T>(T value) where T : struct
		{
			if(!TypeData.WriteTypes.Contains(typeof(T))) {
				throw new InvalidOperationException($"System {GetType().Name} tried to write an undeclared global component - '{typeof(T).Name}'.");
			}

			World.Set(value);
		}

		protected ReadOnlySpan<Entity> ReadEntities(bool? active = true)
			=> World.ReadEntities();

		protected void SendMessage<T>(in T message) where T : struct
		{
			if(!TypeData.SendTypes.Contains(typeof(T))) {
				throw new InvalidOperationException($"System {GetType().Name} tried to send an undeclared message - '{typeof(T).Name}'.");
			}

			World.SendMessage(message);
		}

		protected ReadOnlySpan<T> ReadMessages<T>() where T : struct
		{
			if(!TypeData.ReceiveTypes.Contains(typeof(T))) {
				throw new InvalidOperationException($"System {GetType().Name} tried to read an undeclared message - '{typeof(T).Name}'.");
			}

			return World.ReadMessages<T>();
		}
	}
}
