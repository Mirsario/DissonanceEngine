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
				light.Transform.parent = Transform;
				light.Transform.LocalPosition = new Vector3(0f,1f,0f);

				audioSource = AddComponent<AudioSource>();
				audioSource.Clip = Resources.Get<AudioClip>("Sounds/FireLoop.ogg");
				audioSource.Loop = true;
				audioSource.Play();
			}
		}
		public override void FixedUpdate()
		{
			
		}
		public override void RenderUpdate()
		{
			light.range = Rand.Range(11f,17f);
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<WoodPhysicMaterial>();
	}
}
