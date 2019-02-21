using GameEngine;

namespace SurvivalGame
{
	public class DirtPhysicMaterial : PhysicMaterial
	{
		public override void GetFootstepInfo(Vector3 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants)
		{
			surfaceType = "Dirt";
			switch(actionType) {
				case "Land":
					numSoundVariants = 4;
					break;
				case "Run":
				case "Walk":
					numSoundVariants = 11;
					break;
				default:
					actionType = "Walk";
					numSoundVariants = 11;
					break;
			}
		}
	}
}
