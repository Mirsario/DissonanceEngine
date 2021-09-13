using System;

namespace Dissonance.Engine
{
	[Flags]
	public enum GameFlags
	{
		Default = 0,
		/// <summary> Prevents the game from creating a window. <para/> If set without a <seealso cref="NoGraphics"/> flag, it'll be expected that a GLFW/OpenGL context has been associated with the game's thread. </summary>
		NoWindow = 1,
		/// <summary> Prevents the game from doing any rendering. </summary>
		NoGraphics = 2,
		/// <summary> Prevents the game from initializing and playing audio. </summary>
		NoAudio = 4,
		/// <summary> Makes <see cref="Game.Run(GameFlags, string[])"/> calls skip the update loop. This flag is usable for running games inside editors. <para/> Use <see cref="Game.Update()"/> to actually update the game, and don't forget to dispose it. </summary>
		ManualUpdate = 8,
	}
}
