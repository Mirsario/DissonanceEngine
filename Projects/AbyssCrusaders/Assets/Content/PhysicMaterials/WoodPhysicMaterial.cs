using GameEngine;

namespace AbyssCrusaders.Content.PhysicMaterials
{
	public class WoodPhysicMaterial : PhysicMaterial
	{
		public override void GetFootstepInfo(Vector2 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants)
		{
			actionType = "Walk";
			surfaceType = "Wood";
			numSoundVariants = 11;
		}
		public override string GetSound(string type)
		{
			return type switch
			{
				"Hit" => $"WoodHit.ogg",
				"Break" => $"WoodBreak.ogg",
				"Place" => $"WoodPlace.ogg",
				"" => null,
				_ => null
			};
		}
	}
}
