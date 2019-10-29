using System;

namespace AbyssCrusaders
{
	[Flags]
	public enum NetMode
	{
		//6th-is multiplayer?
		//7th-is the host?
		//8th-is a client?
		Singleplayer = 3,	//00000011
		MultiplayerClient = 5,	//00000101
		DedicatedServer = 6,	//00000110
		MultiplayerHostClient = 7,	//00000111
	}

	public static class Netplay
	{
		public static bool isClient = true;	//Does this instance of the game need to do rendering & audio?
		public static bool isHost = true; //Does this instance of the game own the currently loaded worlds?
		public static bool isMultiplayer; //Does this instance of the game have to deal with networking?

		private static NetMode netMode = NetMode.Singleplayer;

		public static NetMode NetMode {
			get => netMode;
			set {
				netMode = value;
				isClient = NetMode!=NetMode.DedicatedServer;
				isHost = NetMode!=NetMode.MultiplayerClient;
				isMultiplayer = NetMode!=NetMode.Singleplayer;
			}
		}
	}
}
