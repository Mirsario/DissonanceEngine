using System;

namespace Dissonance.Engine.Core
{
	[Flags]
	public enum GameFlags
	{
		None = 0,
		/// <summary> Prevents the game from creating a window. <para/> If set without a <seealso cref="NoGraphics"/> flag, it'll be expected that a GLFW/OpenGL context has been associated with the game's thread. </summary>
		NoWindow = 1,
		/// <summary> Prevents the game from doing any rendering. </summary>
		NoGraphics = 2,
		/// <summary> Prevents the game from initializing and playing audio. </summary>
		NoAudio = 4
	}
}
