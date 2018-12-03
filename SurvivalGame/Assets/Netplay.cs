using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
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
		private static NetMode _netMode = NetMode.Singleplayer;
		public static NetMode NetMode {
			get => _netMode;
			set {
				_netMode = value;
				isClient = NetMode!=NetMode.DedicatedServer;	
				isHost = NetMode!=NetMode.MultiplayerClient;	
				isMultiplayer = NetMode!=NetMode.Singleplayer;		
			}
		}
		public static bool isClient = true;	//Does this instance of the game need to do rendering & audio?
		public static bool isHost = true; //Does this instance of the game own the currently loaded worlds?
		public static bool isMultiplayer; //Does this instance of the game have to deal with networking?
	}
}
