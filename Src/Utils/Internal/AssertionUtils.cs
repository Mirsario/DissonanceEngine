using System;

namespace Dissonance.Engine.Utils
{
	internal static class AssertionUtils
	{
		public static void TypeIsAssignableFrom(Type type, Type otherType)
		{
			if(!type.IsAssignableFrom(otherType)) {
				throw new ArgumentException($"Type '{otherType.Name}' does not derive from '{type.Name}'.");
			}
		}
	}
}
