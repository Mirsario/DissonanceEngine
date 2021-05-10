using System;

namespace Dissonance.Engine
{
	public struct DependencyInfo
	{
		public Type type;
		public bool optional;

		public DependencyInfo(Type type, bool optional = false)
		{
			this.type = type;
			this.optional = optional;
		}
	}
}
