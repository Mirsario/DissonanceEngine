using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class Campfire : StaticEntity, IHasMaterial
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

				light = Instantiate<LightObj>(world).GetComponent<Light>();
				light.color = new Vector3(1f,0.8f,0f);
				light.intensity = 2f;
				light.Transform.parent = Transform;
				light.Transform.LocalPosition = new Vector3(0f,1.25f,0f);

				/*audioSource = AddComponent<AudioSource>();
				var clip = Resources.Get<AudioClip>("Sounds/FireLoop.ogg");
				audioSource.Clip = clip;
				audioSource.PlaybackOffset = Rand.Next(clip.LengthInSeconds);
				audioSource.Loop = true;
				audioSource.RefDistance = 0.1f;
				audioSource.Play();*/
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
			light.range = Mathf.Lerp(light.range,Mathf.Lerp(10f,15f,lightScale),Time.RenderDeltaTime*4f);
			light.intensity = Mathf.Lerp(light.intensity,Mathf.Lerp(0.75f,1.75f,lightScale),Time.RenderDeltaTime*4f);
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<WoodPhysicMaterial>();
	}
}
