using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine
{
	public sealed class PackedEntity
	{
		private static MethodInfo EntitySetComponentMethod = typeof(Entity).GetMethod(nameof(Entity.Set));

		private readonly Dictionary<Type, object> PackedComponents = new();

		public T GetComponent<T>() where T : struct, IComponent
			=> (T)PackedComponents[typeof(T)];

		public bool HasComponent<T>() where T : struct, IComponent
			=> PackedComponents.ContainsKey(typeof(T));

		public void SetComponent<T>(T value) where T : struct, IComponent
			=> PackedComponents[typeof(T)] = value;

		public Entity Unpack(World world)
		{
			var entity = world.CreateEntity();

			foreach(var pair in PackedComponents) {
				//TODO: Very temporary implementation.
				EntitySetComponentMethod
					.MakeGenericMethod(pair.Key)
					.Invoke(entity, new[] { pair.Value });
			}

			return entity;
		}
	}
}
