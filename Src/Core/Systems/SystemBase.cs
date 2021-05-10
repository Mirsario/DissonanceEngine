using System.Linq;
using System.Reflection;

namespace Dissonance.Engine
{
	public abstract class SystemBase
	{
		public DependencyInfo[] Dependencies { get; internal set; }

		public SystemBase()
		{
			Dependencies = GetType()
				.GetCustomAttributes<SystemDependencyAttribute>()
				.SelectMany(a => a.Dependencies)
				.ToArray();
		}

		public virtual void FixedUpdate() { }
		public virtual void RenderUpdate() { }
	}
}
