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
			void GetTypesFromAttribute<T>(HashSet<Type> hashSet) where T : SystemTypesAttribute
			{
				var attrib = type.GetCustomAttribute<T>();

				if(attrib != null) {
					hashSet.UnionWith(attrib.Types);
				}
			}

			GetTypesFromAttribute<ReadsAttribute>(ReadTypes);
			GetTypesFromAttribute<WritesAttribute>(WriteTypes);
			GetTypesFromAttribute<ReceivesAttribute>(ReceiveTypes);
			GetTypesFromAttribute<SendsAttribute>(SendTypes);
		}
	}
}
