using System;

namespace Dissonance.Engine.Core.Modules
{
	public struct ModuleDependency
	{
		public Type type;
		public bool optional;

		public ModuleDependency(Type type, bool optional = false)
		{
			this.type = type;
			this.optional = optional;
		}
	}
}
