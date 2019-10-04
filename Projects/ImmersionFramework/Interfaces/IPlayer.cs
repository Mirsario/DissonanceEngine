using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public interface IPlayer
	{
		int Id { get; } //Self-explanatory.
		bool IsLocal { get; } //Returns if player is local. Note that there may be more than 1 local player, if playing local coop.
		 //int LocalId { get; } //For cases when players mix local & online multiplayer.
	}
}
