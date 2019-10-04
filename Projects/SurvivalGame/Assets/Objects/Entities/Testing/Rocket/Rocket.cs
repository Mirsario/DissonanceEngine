using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;
using ImmersionFramework;

namespace SurvivalGame
{
	public class Rocket : Entity
	{
		public MeshRenderer renderer;
		public AudioSource audioSource;
		public Vector3 velocity;
		public Light light;

		public Entity owner;

		public override void OnInit()
		{
			Transform.LocalScale *= 0.5f;

			renderer = AddComponent<MeshRenderer>(c => {
				c.Mesh = PrimitiveMeshes.Cube;
				c.Material = Resources.Get<Material>("Entities/Testing/Rocket/Rocket.material");
			});

			light = AddComponent<Light>(c => c.color = new Vector3(1f,0.75f,0f));
			
			(audioSource = AddComponent<AudioSource>(c => {
				c.Clip = Resources.Get<AudioClip>("Sounds/rocketFly.wav");
				c.Loop = true;
				c.Volume = 2f;
			})).Play();
		}
		public override void FixedUpdate()
		{
			if(velocity==Vector3.Zero) {
				return;
			}

			Transform.Rotation = Quaternion.FromDirection(velocity.Normalized,Vector3.Up);

			var deltaVelocity = velocity*Time.FixedDeltaTime;
			if(PhysicsEngine.Raycast(Transform.Position,deltaVelocity.Normalized,out var hit,deltaVelocity.Magnitude,customFilter:o => o==owner ? new bool?(false) : null)) {
				Transform.Position = hit.point;
				const float maxDistance = 10f;
				const float power = 3000f;
				SoundInstance.Create("Explosion"+Rand.Range(1,3)+".ogg",Transform.Position,10f);
				ScreenShake.New(0.3f,1f,maxDistance*5f,Transform.Position);

				foreach(var rigidbodyBase in PhysicsEngine.ActiveRigidbodies) {
					if(rigidbodyBase is Rigidbody body) {
						var direction = body.Transform.Position-Transform.Position;
						if(direction==Vector3.Zero) {
							continue;
						}
						float distance = direction.Magnitude;
						direction.Normalize();
						float powerScale = distance/maxDistance;
						if(powerScale>=1f) {
							continue;
						}
						powerScale = 1f-powerScale;
						body.ApplyForce(direction*power*powerScale,Vector3.Zero);
					}
				}

				audioSource.Stop();
				Dispose();
			}else{
				Transform.Position += deltaVelocity;
			}
		}
	}
}