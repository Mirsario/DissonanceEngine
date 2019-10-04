using GameEngine;
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
			var player = new LocalPlayer(0);

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
	}
}
