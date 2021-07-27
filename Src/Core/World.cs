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

		public bool RemoveEntity(Entity entity)
			=> EntityManager.RemoveEntity(entity);

		public EntitySet GetEntitySet(Expression<Predicate<Entity>> expression)
			=> GetEntitySet(EntitySetType.Default, expression);

		public EntitySet GetEntitySet(EntitySetType type, Expression<Predicate<Entity>> expression)
			=> EntityManager.GetEntitySet(Id, type, expression);

		internal ReadOnlySpan<Entity> ReadEntities(bool? active = true)
			=> EntityManager.ReadEntities(Id, active);

		// Systems

		public void AddSystem(GameSystem system)
			=> SystemManager.AddSystemToWorld(this, system);

		// Components

		internal bool Has<T>() where T : struct
			=> ComponentManager.HasComponent<T>(Id);

		internal ref T Get<T>() where T : struct
			=> ref ComponentManager.GetComponent<T>(Id);

		internal void Set<T>(T value) where T : struct
			=> ComponentManager.SetComponent(Id, value);

		// Messages

		internal ReadOnlySpan<T> ReadMessages<T>() where T : struct
			=> MessageManager.ReadMessages<T>(Id);

		internal void SendMessage<T>(in T message) where T : struct
			=> MessageManager.SendMessage(Id, message);
	}
}
