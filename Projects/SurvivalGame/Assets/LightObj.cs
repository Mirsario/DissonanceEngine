using GameEngine;

namespace SurvivalGame
{
	public class LightObj : Entity
	{
		public Light light;

		public override void OnInit() => light = AddComponent<Light>(c => {
			c.range = 128f;
			c.color = new Vector3(Rand.Range(0f,1f),Rand.Range(0f,1f),Rand.Range(0f,1f)).Normalized;
		});
	}
}