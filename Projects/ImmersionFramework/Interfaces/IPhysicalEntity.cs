using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public interface IPhysicalEntity
	{
		Vector3 Velocity { get; set; }
		Vector3 PrevVelocity { get; set; }
	}
}
