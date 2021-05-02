using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine
{
	[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public abstract class ComponentAttribute : Attribute
	{
		private static Dictionary<Type, ComponentAttribute[]> cache = new Dictionary<Type, ComponentAttribute[]>();

		public virtual void PreAddComponent(GameObject gameObject, Type type) { }
		public virtual void OnComponentEnabled(GameObject gameObject, IComponent component) { }
		public virtual void OnComponentDisabled(GameObject gameObject, IComponent component) { }

		public static IEnumerable<ComponentAttribute> EnumerateForType(Type type)
		{
			if(!cache.TryGetValue(type, out var array)) {
				cache[type] = array = type.GetCustomAttributes<ComponentAttribute>(true).ToArray();
			}

			return array;
		}
	}
}
