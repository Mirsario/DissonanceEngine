using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class Sun : GameObject
	{
		public Light light;

		public override void OnInit()
		{
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
		public override void RenderUpdate()
		{

			Rendering.ambientColor = new Vector3(0.2f);
			var t = Transform;

			t.EulerRot = new Vector3(60f,30f,0f);

			//TODO:
			/*if(Main.camera!=null) {
				t.Position = Main.camera.Transform.Position-Transform.Forward*100f;
			}*/
        }
	}
}