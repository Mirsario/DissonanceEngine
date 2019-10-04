using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public interface IClient
	{
		IPlayer[] Players { get; } //Players currently playing from this client.
	}

	public interface IClient<TPlayer> : IClient where TPlayer : IPlayer
	{
		new TPlayer[] Players { get; } //Players currently playing from this client.
	}
}
