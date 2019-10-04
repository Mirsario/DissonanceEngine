using GameEngine;
using ImmersionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class Client : IClient<Player>
	{
		public Player[] Players => throw new NotImplementedException();

		IPlayer[] IClient.Players => Players;
	}
}
