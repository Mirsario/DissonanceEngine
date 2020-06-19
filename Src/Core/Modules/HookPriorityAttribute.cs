using System;

namespace Dissonance.Engine.Core.Modules
{
	public class HookPositionAttribute : Attribute
	{
		public readonly int Position;

		public HookPositionAttribute(int position)
		{
			Position = position;
		}
	}
}
