using AbyssCrusaders.Core;
using GameEngine;

namespace AbyssCrusaders.Core
{
	public class LightObj : Entity
	{
		public Light2D light;

		public override void OnInit()
		{
			light = AddComponent<Light2D>(c => {
				c.range = 32f;
				c.color = new Vector3(Rand.Range(0f,1f),Rand.Range(0f,1f),Rand.Range(0f,1f)).Normalized;
			});
		}
	}
}