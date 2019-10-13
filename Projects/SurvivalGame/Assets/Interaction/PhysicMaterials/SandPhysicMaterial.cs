using GameEngine;

namespace SurvivalGame
{
	public class SandPhysicMaterial : PhysicMaterial
	{
		public override void GetFootstepInfo(Vector3 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants)
		{
			surfaceType = "Sand";

			if(actionType!="Walk" && actionType!="Run") {
				actionType = "Run";
			}

			numSoundVariants = 11;
		}
	}
}
