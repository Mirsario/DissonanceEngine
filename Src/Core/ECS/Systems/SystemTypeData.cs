using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine
{
	public sealed class SystemTypeData
	{
		public readonly HashSet<Type> Callbacks = new();

		public SystemTypeData(Type type)
		{
			foreach (var attribute in type.GetCustomAttributes<SystemTypeDataAttribute>()) {
				attribute.ModifySystemTypeData(this);
			}
		}
	}
}
