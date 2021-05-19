using System;

namespace Dissonance.Engine
{
	public sealed class ReadsAttribute : Attribute
	{
		public readonly Type[] Types;

		public ReadsAttribute(params Type[] types)
		{
			Types = types;
		}
	}
}
