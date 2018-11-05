using System;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	public class Sun : GameObject
	{
		public Light light;
		public override void OnInit()
		{
			light = AddComponent<Light>();
			light.type = LightType.Directional;

			light.color = new Vector3(1f,0.85f,0.75f);		//Sun
			Graphics.ambientColor = Vector3.one*0.25f;
			//light.color = new Vector3(0.66f,0.33f,1f)*0.25f;	//Moon
			//Graphics.ambientColor = new Vector3(0.05f);

			/*MeshRenderer renderer = AddComponent<MeshRenderer>();
			renderer.mesh = Graphics.sphereMesh;
			renderer.material = Resources.Get<Material>("Boulder.material");*/
		}
		public override void RenderUpdate()
		{
			Transform.EulerRot = new Vector3(-60f,0f,0f);
			if(Main.camera!=null) {
				Transform.Position = Main.camera.Transform.Position+new Vector3(20f,40f,10f);
			}
			Transform.LocalScale = Vector3.one*10f;
		}
	}
}