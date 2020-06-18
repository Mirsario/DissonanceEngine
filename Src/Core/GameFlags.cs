using System;

namespace Dissonance.Engine.Core
{
	[Flags]
	public enum GameFlags
	{
		None,
		NoGraphics = 1,
		NoAudio = 2
	}
}
