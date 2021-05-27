using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public sealed class World
	{
		internal readonly List<Entity> Entities = new();
		internal readonly List<int> FreeEntityIndices = new();
		internal readonly List<GameSystem> Systems = new();
		internal readonly int Id;

		private readonly List<EntitySet> EntitySets = new();
		private readonly Dictionary<EntitySetType, Dictionary<Expression<Predicate<Entity>>, EntitySet>> EntitySetsQuery = new();

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

			setsByExpressions[predicate] = entitySet;

			EntitySets.Add(entitySet);

			return entitySet;
		}

		public void AddSystem(GameSystem gameSystem)
		{
			Systems.Add(gameSystem);
		}

		internal bool Has<T>() where T : struct, IComponent
			=> ComponentManager.HasComponent<T>(Id);

		internal ref T Get<T>() where T : struct, IComponent
			=> ref ComponentManager.GetComponent<T>(Id);

		internal void Set<T>(T value) where T : struct, IComponent
			=> ComponentManager.SetComponent(Id, value);

		internal ReadOnlySpan<Entity> ReadEntities()
			=> CollectionsMarshal.AsSpan(Entities);

		internal void FixedUpdate()
		{
			foreach(var system in Systems) {
				system.FixedUpdate();
			}
		}

		internal void RenderUpdate()
		{
			foreach(var system in Systems) {
				system.RenderUpdate();
			}
		}

		internal void OnEntityUpdated(Entity entity)
		{
			foreach(var entitySet in EntitySets) {
				entitySet.OnEntityUpdated(entity);
			}
		}
	}
}
