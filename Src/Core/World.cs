using System;
using System.Linq.Expressions;

namespace Dissonance.Engine
{
	public sealed class World
	{
		internal readonly int Id;

		internal World(int id)
		{
			Id = id;
		}

		// Entities

		public Entity CreateEntity()
			=> EntityManager.CreateEntity(Id);

		public EntitySet GetEntitySet(Expression<Predicate<Entity>> expression)
			=> GetEntitySet(EntitySetType.Default, expression);

		public EntitySet GetEntitySet(EntitySetType type, Expression<Predicate<Entity>> expression)
			=> EntityManager.GetEntitySet(Id, type, expression);

		public ReadOnlySpan<Entity> ReadEntities(bool? active = true)
			=> EntityManager.ReadEntities(Id, active);

		// Systems

		public void AddSystem(GameSystem system)
			=> SystemManager.AddSystemToWorld(this, system);

		// Components

		public bool Has<T>() where T : struct
			=> ComponentManager.HasComponent<T>(Id);

		public ref T Get<T>() where T : struct
			=> ref ComponentManager.GetComponent<T>(Id);

		public void Set<T>(T value) where T : struct
			=> ComponentManager.SetComponent(Id, value);

		// Messages

		public ReadOnlySpan<T> ReadMessages<T>() where T : struct
			=> MessageManager.ReadMessages<T>(Id);

		public void SendMessage<T>(in T message) where T : struct
			=> MessageManager.SendMessage(Id, message);
	}
}
