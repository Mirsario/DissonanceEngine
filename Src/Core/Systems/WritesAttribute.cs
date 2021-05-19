using System;

namespace Dissonance.Engine
{
	public sealed class WritesAttribute : Attribute
	{
		public readonly Type Type;

		public WritesAttribute(Type type)
		{
			Type = type;
		}
	}
}
