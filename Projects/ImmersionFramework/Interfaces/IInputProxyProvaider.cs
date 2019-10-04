using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public interface IInputProxyProvaider : IInputProvaider
	{
		public InputProxy Proxy { get; }
	}
}
