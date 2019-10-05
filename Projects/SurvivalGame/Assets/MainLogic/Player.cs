using GameEngine;
using GameEngine.Utils;
using ImmersionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class Player : PlayerBase<Player,LocalPlayer,PhysicalEntity> //, IInputSource<InputSignal>
	{
		public override bool IsLocal => false;

		public Player(int id) : base(id) {}

		public static void Initialize()
		{
			var player = new LocalPlayer(0,0);

			players = new Player[] { player };
			localPlayers = new LocalPlayer[] { player };
		}
		public static void PlayerFixedUpdate()
		{
			for(int i = 0;i<players.Length;i++) {
				players[i].FixedUpdate();
			}
		}
		public static void PlayerRenderUpdate()
		{
			for(int i = 0;i<players.Length;i++) {
				players[i].RenderUpdate();
			}
		}

		public static Player AddPlayer(Player player)
		{
			ArrayUtils.Add(ref players,player);
			return player;
		}
		public static LocalPlayer AddLocalPlayer() => AddLocalPlayer(new LocalPlayer(PlayerCount,LocalPlayerCount));
		public static LocalPlayer AddLocalPlayer(LocalPlayer localPlayer)
		{
			ArrayUtils.Add(ref localPlayers,localPlayer);
			AddPlayer(localPlayer);
			return localPlayer;
		}
		public static void RemovePlayer(Player player)
		{
			ArrayUtils.Remove(ref players,player.Id);

			if(player is LocalPlayer localPlayer) {
				ArrayUtils.Remove(ref localPlayers,localPlayer.LocalId);
			}
		}
	}
}
