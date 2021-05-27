using System;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ReadsAttribute : Attribute
	{
		public readonly Type[] Types;

		public ReadsAttribute(params Type[] types)
		{
			AssertionUtils.ValuesNotNull(types, nameof(types));
			AssertionUtils.TypesAreStruct(types);
			AssertionUtils.TypesHaveInterface(types, typeof(IComponent));

			Types = types;
		}
	}
}
