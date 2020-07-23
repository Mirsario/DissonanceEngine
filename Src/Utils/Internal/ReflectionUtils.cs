using System;
using System.Collections.Generic;

namespace Dissonance.Engine.Utils.Internal
{
	internal static class ReflectionUtils
	{
		public static IEnumerable<Type> EnumerateBaseTypes(Type type,bool includingOriginal = false,Type stopAt = null)
		{
			if(!includingOriginal) {
				type = type.BaseType;
			}

			while(type!=stopAt) {
				yield return type;

				type = type.BaseType;
			}
		}
	}
}
