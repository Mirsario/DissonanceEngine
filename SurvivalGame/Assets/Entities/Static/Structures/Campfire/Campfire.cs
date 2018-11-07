using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;

namespace Game
{
	public class Campfire : StaticEntity,IHasMaterial
	{
		public MeshRenderer renderer;
		public MeshCollider collider;
		public Light light;
		public AudioSource audioSource;
		private float lightScale;
		
		public override void OnInit()
		{
			base.OnInit();
			
			var mesh = Resources.Get<Mesh>($"{GetType().Name}.obj");

			collider = AddComponent<MeshCollider>();
			collider.Mesh = mesh;
			collider.Convex = true;

			if(Netplay.isClient) {
				renderer = AddComponent<MeshRenderer>();
				renderer.Mesh = mesh;
				renderer.Material = Resources.Find<Material>($"{GetType().Name}");

				light = Instantiate<LightObj>().GetComponent<Light>();
				light.color = new Vector3(1f,0.8f,0f);
				light.intensity = 2f;
				light.Transform.parent = Transform;
				light.Transform.LocalPosition = new Vector3(0f,1f,0f);

				audioSource = AddComponent<AudioSource>();
				audioSource.Clip = Resources.Get<AudioClip>("Sounds/FireLoop.ogg");
				audioSource.Loop = true;
				audioSource.Volume = 0.3f;
				audioSource.Play();
			}
		}
		public override void FixedUpdate()
		{
			if(Time.FixedUpdateCount%(Time.TargetUpdateCount/6)==0) {
				lightScale = Rand.Range(0f,1f);
			}
		}
		public override void RenderUpdate()
		{
			light.range = Mathf.Lerp(light.range,Mathf.Lerp(10f,15f,lightScale),Time.DeltaTime*4f);
			light.intensity = Mathf.Lerp(light.intensity,Mathf.Lerp(0.75f,1.75f,lightScale),Time.DeltaTime*4f);
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<WoodPhysicMaterial>();
	}
}
