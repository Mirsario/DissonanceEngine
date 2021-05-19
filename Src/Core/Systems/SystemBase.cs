using System;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine
{
	public abstract class SystemBase
	{
		public DependencyInfo[] Dependencies { get; private set; }

		internal World World { get; set; }

		internal SystemBase()
		{
			Dependencies = GetType()
				.GetCustomAttributes<SystemDependencyAttribute>()
				.SelectMany(a => a.Dependencies)
				.ToArray();
		}

		public ReadOnlySpan<Entity> ReadEntities()
		{
			if(World != null) {
				return World.ReadEntities();
			}

			return EntityManager.ReadAllEntities();
		}
	}
}
