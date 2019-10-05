using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class Sun : GameObject
	{
		private static Sun instance;

		public Light light;

		public override void OnInit()
		{
			instance = this;

			light = AddComponent<Light>(c => {
				c.type = LightType.Directional;
				c.intensity = 1.5f;
				c.color = new Vector3(1f,0.85f,0.75f); //Sun
				//c.color = new Vector3(0.66f,0.33f,1f)*0.25f; //Moon
			});

			AddComponent<MeshRenderer>(c => {
				c.Mesh = PrimitiveMeshes.Sphere;
				c.Material = Resources.Get<Material>("Sun.material");
			});
		}

		public static void OnRenderStart(Camera camera)
		{
			Rendering.ambientColor = new Vector3(0.2f);

			if(instance!=null) {
				var t = instance.Transform;
				t.EulerRot = new Vector3(60f,30f,0f);
				t.Position = camera.Transform.Position-t.Forward*100f;
			}
		}
	}
}