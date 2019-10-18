using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public class Campfire : StaticEntity, IHasMaterial
	{
		public MeshRenderer renderer;
		public MeshCollider collider;
		public Light light;
		public AudioSource audioSource;
		private float lightScale;

		public override CollisionMesh CollisionMesh => Resources.Get<ConvexCollisionMesh>($"{GetType().Name}.obj");
		public override (Mesh mesh, Material material)[] RendererData => new[] {
			(Resources.Get<Mesh>($"{GetType().Name}.obj"),Resources.Find<Material>($"{GetType().Name}"))
		};

		public override void OnInit()
		{
			base.OnInit();
			
			if(Netplay.isClient) {
				light = Instantiate<LightObj>(world).GetComponent<Light>();
				light.color = new Vector3(1f,0.8f,0f);

				light.Transform.parent = Transform;
				light.Transform.LocalPosition = new Vector3(0f,1.25f,0f);

				/*audioSource = AddComponent<AudioSource>(s => {
					var clip = Resources.Get<AudioClip>("Sounds/FireLoop.ogg");

					s.Clip = clip;
					s.Loop = true;
					s.MaxDistance = 64f;
					s.Volume = 0.2f;
					s.PlaybackOffset = Rand.Next(clip.LengthInSeconds);
				});

				audioSource.Play();*/
			}
		}
		public override void FixedUpdate()
		{
			if(Time.FixedUpdateCount%(Time.TargetUpdateFrequency/6)==0) {
				lightScale = Rand.Range(0f,1f);
			}
		}
		public override void RenderUpdate()
		{
			light.range = Mathf.Lerp(light.range,Mathf.Lerp(10f,15f,lightScale),Time.RenderDeltaTime*4f);
			light.intensity = Mathf.Lerp(light.intensity,Mathf.Lerp(1f,3f,lightScale),Time.RenderDeltaTime*4f);
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<WoodPhysicMaterial>();
	}
}
