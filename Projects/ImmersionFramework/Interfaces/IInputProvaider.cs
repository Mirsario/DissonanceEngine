using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public interface IInputProvaider
	{
		public InputSignal[] Inputs { get; }
		public Vector3 LookDirection { get; }
		public Vector3 LookRotation { get; }
	}
}
