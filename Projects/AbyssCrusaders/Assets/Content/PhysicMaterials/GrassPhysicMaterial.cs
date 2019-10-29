using GameEngine;

namespace AbyssCrusaders.Content.PhysicMaterials
{
	public class GrassPhysicMaterial : PhysicMaterial
	{
		public override void GetFootstepInfo(Vector2 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants)
		{
			surfaceType = "Grass";

			if(actionType=="Land" || actionType=="Run") {
				actionType = "Run";
				numSoundVariants = 4;
			} else {
				actionType = "Walk";
				numSoundVariants = 10;
			}
		}
	}
}
