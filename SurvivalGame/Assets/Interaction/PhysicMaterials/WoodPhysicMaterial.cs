using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine;

namespace Game
{
	public class WoodPhysicMaterial : PhysicMaterial
	{
		public override void GetFootstepInfo(Vector3 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants)
		{
			actionType = "Walk";
			surfaceType = "Wood";
			numSoundVariants = 11;
		}
	}
}
