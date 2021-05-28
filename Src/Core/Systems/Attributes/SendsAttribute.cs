using System;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public sealed class SendsAttribute : SystemTypesAttribute
	{
		public SendsAttribute(params Type[] types)
		{
			AssertionUtils.ValuesNotNull(types, nameof(types));
			AssertionUtils.TypesAreStruct(types);
			AssertionUtils.TypesHaveInterface(types, typeof(IMessage));

			Types = types;
		}
	}
}
