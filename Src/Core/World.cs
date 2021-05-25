using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public sealed class World
	{
		internal readonly List<Entity> Entities = new List<Entity>();
		internal readonly List<int> FreeEntityIndices = new List<int>();
		internal readonly List<RenderSystem> RenderSystems = new List<RenderSystem>();
		internal readonly List<GameSystem> GameSystems = new List<GameSystem>();

		internal readonly int Id;

		internal World(int id)
		{
			Id = id;
		}

		public Entity CreateEntity()
		{
			return EntityManager.CreateEntity(this);
		}

		public void AddSystem(GameSystem gameSystem)
		{
			GameSystems.Add(gameSystem);
		}

		public void AddSystem(RenderSystem renderSystem)
		{
			RenderSystems.Add(renderSystem);
		}

		public bool Has<T>() where T : struct, IComponent
			=> ComponentManager.HasComponent<T>(Id);

		public ref T Get<T>() where T : struct, IComponent
			=> ref ComponentManager.GetComponent<T>(Id);

		public void Set<T>(T value) where T : struct, IComponent
			=> ComponentManager.SetComponent(Id, value);

		public ReadOnlySpan<Entity> ReadEntities()
			=> CollectionsMarshal.AsSpan(Entities);

		internal void FixedUpdate()
		{
			foreach(var system in GameSystems) {
				system.Update();
			}
		}
		internal void RenderUpdate()
		{
			foreach(var system in RenderSystems) {
				system.Update();
			}
		}
	}
}
