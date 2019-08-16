using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class Sun : GameObject
	{
		public Light light;

		public override void OnInit()
		{
			light = AddComponent<Light>();
			light.type = LightType.Directional;

			light.color = new Vector3(1f,0.85f,0.75f); //Sun
			light.intensity = 1.5f;
			Rendering.ambientColor = Vector3.Zero;
			//light.color = new Vector3(0.66f,0.33f,1f)*0.25f; //Moon
			//Rendering.ambientColor = new Vector3(0.05f);

			MeshRenderer renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.Sphere;
			renderer.Material = Resources.Get<Material>("Sun.material");
		}
		public override void RenderUpdate()
		{
            var t = Transform;
			var camera = Main.camera;
			t.EulerRot = new Vector3(60f,30f,0f);
			if(camera!=null) {
				t.Position = camera.Transform.Position-Transform.Forward*100f;
			}
        }
	}
}