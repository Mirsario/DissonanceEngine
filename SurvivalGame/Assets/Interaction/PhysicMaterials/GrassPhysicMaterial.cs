using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine;

namespace Game
{
	public class GrassPhysicMaterial : PhysicMaterial
	{
		public override void GetFootstepInfo(Vector3 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants)
		{
			surfaceType = "Grass";
			if(actionType=="Land" || actionType=="Run") {
				actionType = "Run";
				numSoundVariants = 4;
			}else{
				actionType = "Walk";
				numSoundVariants = 10;
			}
		}
	}
}
