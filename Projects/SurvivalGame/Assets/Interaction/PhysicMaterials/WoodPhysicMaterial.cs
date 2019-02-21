using GameEngine;

namespace SurvivalGame
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
