using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public interface ILocalPlayer
	{
		int LocalId { get; } //For cases when players mix local & online multiplayer.
		CameraController Camera { get; }
	}
}
