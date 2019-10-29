using GameEngine;

namespace AbyssCrusaders.Content.PhysicMaterials
{
	public class SandPhysicMaterial : PhysicMaterial
	{
		public override void GetFootstepInfo(Vector2 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants)
		{
			surfaceType = "Sand";

			if(actionType!="Walk" && actionType!="Run") {
				actionType = "Run";
			}

			numSoundVariants = 11;
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
