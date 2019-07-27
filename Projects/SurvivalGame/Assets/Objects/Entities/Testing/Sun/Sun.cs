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
			Rendering.ambientColor = Vector3.one*0.25f;
			//light.color = new Vector3(0.66f,0.33f,1f)*0.25f; //Moon
			//Graphics.ambientColor = new Vector3(0.05f);

			MeshRenderer renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.Sphere;
			renderer.Material = Resources.Get<Material>("Sun.material");
		}
		public override void RenderUpdate()
		{
            var t = Transform;
			var camera = Main.camera;
			if(camera!=null) {
				t.Position = camera.Transform.Position+new Vector3(20f,40f,10f);
			}
			t.EulerRot = new Vector3(-60f,0f,0f);
			t.LocalScale = Vector3.one;
        }
	}
}