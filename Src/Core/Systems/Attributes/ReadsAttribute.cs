using System;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public sealed class ReadsAttribute : SystemTypesAttribute
	{
		public ReadsAttribute(params Type[] types)
		{
			AssertionUtils.ValuesNotNull(types, nameof(types));
			AssertionUtils.TypesAreStruct(types);

			Types = types;
		}
	}
}
