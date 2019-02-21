using GameEngine;

namespace SurvivalGame
{
	public class LightObj : Entity
	{
		public Light light;
		public override void OnInit()
		{
			light = AddComponent<Light>();
			light.range = 16f;
			light.color = new Vector3(Rand.Range(0f,1f),Rand.Range(0f,1f),Rand.Range(0f,1f)).Normalized;
		}
	}
}