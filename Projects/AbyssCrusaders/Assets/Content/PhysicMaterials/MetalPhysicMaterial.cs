using GameEngine;

namespace AbyssCrusaders.Content.PhysicMaterials
{
	public class MetalPhysicMaterial : PhysicMaterial
	{
		public override void GetFootstepInfo(Vector2 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants)
		{
			surfaceType = "Metal";

			switch(actionType) {
				case "Run":
				case "Walk":
					numSoundVariants = 11;
					break;
				case "Land":
					numSoundVariants = 1;
					break;
				default:
					actionType = "Run";
					numSoundVariants = 11;
					break;
			}
		}
		public override string GetSound(string type)
		{
			return type switch
			{
				"Hit" => $"StoneHit{1+Rand.Next(3)}.ogg",
				"Break" => $"StoneBreak.ogg",
				"Place" => $"StonePlace.ogg",
				"" => null,
				_ => null
			};
		}
	}
}
