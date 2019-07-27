using GameEngine;

namespace AbyssCrusaders
{
	public class LightObj : Entity
	{
		public Light2D light;

		public override void OnInit()
		{
			light = AddComponent<Light2D>();
			light.range = 32f;
			light.color = new Vector3(Rand.Range(0f,1f),Rand.Range(0f,1f),Rand.Range(0f,1f)).Normalized;
		}
	}
}