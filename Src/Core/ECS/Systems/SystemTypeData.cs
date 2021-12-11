using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine
{
	public sealed class SystemTypeData
	{
		public readonly Type SystemType;
		public readonly HashSet<Type> Callbacks = new();
		public readonly HashSet<Type> SortingDependencies = new();

		public SystemTypeData(Type type)
		{
			SystemType = type;

			foreach (var attribute in type.GetCustomAttributes<SystemTypeDataAttribute>()) {
				attribute.ModifySystemTypeData(this);
			}
		}
	}
}
