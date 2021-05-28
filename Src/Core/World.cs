using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public sealed class World
	{
		internal readonly int Id;
		internal readonly List<Entity> Entities = new();
		internal readonly List<int> FreeEntityIndices = new();
		internal readonly List<GameSystem> Systems = new();
		internal readonly Dictionary<Type, List<GameSystem>> SystemsByType = new();

		private readonly List<EntitySet> EntitySets = new();
		private readonly Dictionary<EntitySetType, Dictionary<Expression<Predicate<Entity>>, EntitySet>> EntitySetsQuery = new();

		internal bool DefaultSystemsRegistered { get; set; }

		internal World(int id)
		{
			Id = id;
		}

		public Entity CreateEntity()
		{
			var entity = EntityManager.CreateEntity(this);

			OnEntityUpdated(entity);

			return entity;
		}

		public EntitySet GetEntitySet(Expression<Predicate<Entity>> predicate)
			=> GetEntitySet(EntitySetType.Default, predicate);

		public EntitySet GetEntitySet(EntitySetType type, Expression<Predicate<Entity>> predicate)
		{
			if(!EntitySetsQuery.TryGetValue(type, out var setsByExpressions)) {
				EntitySetsQuery[type] = setsByExpressions = new();
			} else {
				//TODO: It seems like Expression.ToString() doesn't at all care about method type parameters.
				/*string expressionString = predicate.ToString();

				foreach(var pair in setsByExpressions) {
					var expression = pair.Key;

					//TODO: Make a more elaborate comparer for expressions. This check has a lot of issues, like variable names mattering.
					if(expression.ToString() == expressionString) {
						return pair.Value;
					}
				}*/
			}

			var entitySet = new EntitySet(type, predicate.Compile());

			foreach(var entity in Entities) {
				entitySet.OnEntityUpdated(entity);
			}

			setsByExpressions[predicate] = entitySet;

			EntitySets.Add(entitySet);

			return entitySet;
		}

		public void AddSystem(GameSystem system)
		{
			var type = system.GetType();

			Systems.Add(system);

			if(!SystemsByType.TryGetValue(type, out var systemsOfThisType)) {
				SystemsByType[type] = systemsOfThisType = new List<GameSystem>();
			}

			systemsOfThisType.Add(system);

			if(DefaultSystemsRegistered) {
				SystemsManager.SortSystems(Systems, SystemsByType);
			}
		}

		internal bool Has<T>() where T : struct, IComponent
			=> ComponentManager.HasComponent<T>(Id);

		internal ref T Get<T>() where T : struct, IComponent
			=> ref ComponentManager.GetComponent<T>(Id);

		internal void Set<T>(T value) where T : struct, IComponent
			=> ComponentManager.SetComponent(Id, value);

		internal ReadOnlySpan<Entity> ReadEntities()
			=> CollectionsMarshal.AsSpan(Entities);

		internal void SendMessage<T>(in T message) where T : struct, IMessage
			=> MessageManager.SendMessage(Id, message);

		internal ReadOnlySpan<T> ReadMessages<T>() where T : struct, IMessage
			=> MessageManager.ReadMessages<T>(Id);

		internal void FixedUpdate()
		{
			foreach(var system in Systems) {
				if(!system.Initialized) {
					system.Initialize();

					system.Initialized = true;
				}

				system.FixedUpdate();
			}

			MessageManager.ClearMessages();
		}

		internal void RenderUpdate()
		{
			foreach(var system in Systems) {
				system.RenderUpdate();
			}

			MessageManager.ClearMessages();
		}

		internal void OnEntityUpdated(Entity entity)
		{
			foreach(var entitySet in EntitySets) {
				entitySet.OnEntityUpdated(entity);
			}
		}
	}
}
