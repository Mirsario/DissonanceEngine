using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;
using ImmersionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class Slug : LivingEntity
	{
		public override Type CameraControllerType => typeof(FirstPersonCamera);

		public override void SetupRenderers(ref Renderer[] renderers)
		{
			renderers = new[] {
				AddComponent<MeshRenderer>(c => {
					c.Mesh = Resources.Get<Mesh>("Slug.obj");
					c.Material = Resources.Get<Material>("Slug.material");
				})
			};
		}
		public override void SetupPhysicsComponents(ref Collider[] colliders,ref Rigidbody rigidbody)
		{

		}
		public override void OnInit()
		{
			base.OnInit();

			if(Netplay.isClient) {
				var audioSource = AddComponent<AudioSource>(s => {
					var clip = Resources.Get<AudioClip>("SlugIdle.ogg");

					s.Clip = clip;
					s.Loop = true;
					s.MaxDistance = 16f;
					s.Volume = 0.25f;
					s.PlaybackOffset = Rand.Next(clip.LengthInSeconds);
				});

				audioSource.Play();
			}
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();

			Transform.Position += new Vector3(this.Value<Inputs.MoveX>(),0f,this.Value<Inputs.MoveY>())*Time.FixedDeltaTime;

			Transform.LocalScale = new Vector3(1f,1f+Mathf.Sin(Time.GameTime*1.5f)/Mathf.FourPI,1f);
		}
	}
}
