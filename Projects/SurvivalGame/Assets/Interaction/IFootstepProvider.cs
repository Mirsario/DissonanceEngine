using GameEngine;

namespace SurvivalGame
{
	public interface IFootstepProvider
	{
		//void GetFootstepInfo(ref string footstepSource,out string surfaceType);
		//TODO: Implement the above arguments instead ^
		void GetFootstepInfo(Vector3 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants);
	}
}