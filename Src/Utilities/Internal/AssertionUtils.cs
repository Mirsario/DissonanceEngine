using System;

namespace Dissonance.Engine.Utilities
{
	internal static class AssertionUtils
	{
		public static void TypeIsAssignableFrom(Type type, Type otherType)
		{
			if (!type.IsAssignableFrom(otherType)) {
				throw new ArgumentException($"Type '{otherType.Name}' does not derive from '{type.Name}'.");
			}
		}

		public static void ValuesNotNull<T>(T[] array, string argName = null) where T : class
		{
			for (int i = 0; i < array.Length; i++) {
				if (array[i] == null) {
					throw new ArgumentNullException($"{argName ?? nameof(array)}[{i}]");
				}
			}
		}

		public static void TypesAreStruct(Type[] array)
		{
			for (int i = 0; i < array.Length; i++) {
				var type = array[i];

				if (type.IsClass || type.IsInterface) {
					throw new ArgumentException($"Type '{type.FullName}' must be a struct.");
				}
			}
		}

		public static void TypesHaveInterface(Type[] array, Type interfaceType)
		{
			for (int i = 0; i < array.Length; i++) {
				var type = array[i];

				if (!interfaceType.IsAssignableFrom(type)) {
					throw new ArgumentException($"Type '{type.FullName}' does not implement interface '{interfaceType.Name}'.");
				}
			}
		}
	}
}
