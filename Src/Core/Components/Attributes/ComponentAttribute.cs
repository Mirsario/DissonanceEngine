using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine
{
	[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public abstract class ComponentAttribute : Attribute
	{
		private static readonly Dictionary<Type, ComponentAttribute[]> Cache = new Dictionary<Type, ComponentAttribute[]>();

		public virtual void PreAddComponent(Entity entity, Type type) { }
		public virtual void OnComponentEnabled(Entity entity, object component) { }
		public virtual void OnComponentDisabled(Entity entity, object component) { }

		public static IEnumerable<ComponentAttribute> EnumerateForType(Type type)
		{
			if(!Cache.TryGetValue(type, out var array)) {
				Cache[type] = array = type.GetCustomAttributes<ComponentAttribute>(true).ToArray();
			}

			return array;
		}
	}
}
