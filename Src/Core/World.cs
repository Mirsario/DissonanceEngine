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

		public Entity CreateEntity()
			=> EntityManager.CreateEntity(Id);

		public bool RemoveEntity(Entity entity)
			=> EntityManager.RemoveEntity(entity);

		public EntitySet GetEntitySet(Expression<Predicate<Entity>> predicate)
			=> GetEntitySet(EntitySetType.Default, predicate);

		public EntitySet GetEntitySet(EntitySetType type, Expression<Predicate<Entity>> predicate)
			=> EntityManager.GetEntitySet(Id, type, predicate);

		public void AddSystem(GameSystem system)
			=> SystemManager.AddSystemToWorld(this, system);

		/*public void AddRenderer(Renderer renderer)
		{
			var type = renderer.GetType();

			Renderers.Add(renderer);

			if(!RenderersByType.TryGetValue(type, out var renderersOfThisType)) {
				RenderersByType[type] = renderersOfThisType = new();
			}

			renderersOfThisType.Add(renderer);
		}*/

		internal bool Has<T>() where T : struct
			=> ComponentManager.HasComponent<T>(Id);

		internal ref T Get<T>() where T : struct
			=> ref ComponentManager.GetComponent<T>(Id);

		internal void Set<T>(T value) where T : struct
			=> ComponentManager.SetComponent(Id, value);

		internal ReadOnlySpan<Entity> ReadEntities(bool? active = true)
			=> EntityManager.ReadEntities(Id, active);

		internal void SendMessage<T>(in T message) where T : struct
			=> MessageManager.SendMessage(Id, message);

		internal ReadOnlySpan<T> ReadMessages<T>() where T : struct
			=> MessageManager.ReadMessages<T>(Id);
	}
}
