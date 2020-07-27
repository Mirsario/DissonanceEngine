using System;

namespace Dissonance.Engine.Core
{
	[Flags]
	public enum GameFlags
	{
		None = 0,
		Default = Graphics|Audio,
		Graphics = 1,
		Audio = 2
	}
}
