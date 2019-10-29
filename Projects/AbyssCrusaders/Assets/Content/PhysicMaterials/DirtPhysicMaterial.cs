using GameEngine;

namespace AbyssCrusaders.Content.PhysicMaterials
{
	public class DirtPhysicMaterial : PhysicMaterial
	{
		public override void GetFootstepInfo(Vector2 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants)
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
		public override string GetSound(string type)
		{
			return type switch
			{
				"Place" => $"DirtHit{1+Rand.Next(3)}.ogg",
				"Hit" => $"DirtHit{1+Rand.Next(3)}.ogg",
				"Break" => $"DirtBreak.ogg",
				"" => null,
				_ => null
			};
		}
	}
}
