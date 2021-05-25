using System;

namespace Dissonance.Engine
{
	public abstract class SystemBase
	{
		public World World { get; internal set; }

		public virtual void Initialize() { }
	}
}
