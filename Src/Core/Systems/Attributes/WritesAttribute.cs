using System;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public sealed class WritesAttribute : SystemTypesAttribute
	{
		public WritesAttribute(params Type[] types)
		{
			AssertionUtils.ValuesNotNull(types, nameof(types));
			AssertionUtils.TypesAreStruct(types);

			Types = types;
		}
	}
}
