using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine
{
	public sealed class SystemTypeData
	{
		public readonly HashSet<Type> ReadTypes = new();
		public readonly HashSet<Type> WriteTypes = new();
		public readonly HashSet<Type> ReceiveTypes = new();
		public readonly HashSet<Type> SendTypes = new();

		public SystemTypeData(Type type)
		{
			foreach (var attribute in type.GetCustomAttributes<SystemTypeDataAttribute>()) {
				attribute.ModifySystemTypeData(this);
			}

			// A bit of hardcode. Receivers of engine-sent ComponentAddedMessage<T> and ComponentRemovedMessage<T> must be treated as readers of T, to depend on writers of it.
			foreach (var receiveType in ReceiveTypes) {
				if (!receiveType.IsConstructedGenericType) {
					continue;
				}

				var typeDefinition = receiveType.GetGenericTypeDefinition();

				if (typeDefinition == typeof(ComponentAddedMessage<>) || typeDefinition == typeof(ComponentRemovedMessage<>)) {
					ReadTypes.Add(receiveType.GetGenericArguments()[0]);
				}
			}
		}
	}
}
