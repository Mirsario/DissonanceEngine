using GameEngine;

namespace SurvivalGame
{
	public class StonePhysicMaterial : PhysicMaterial
	{
		public override void GetFootstepInfo(Vector3 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants)
		{
			actionType = "Walk";
			surfaceType = "Stone";
			numSoundVariants = 11;
		}
	}
}
