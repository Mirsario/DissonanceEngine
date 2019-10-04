using System;
using System.Linq;
using GameEngine;

namespace SurvivalGame
{
	public class AtmosphereSystem : GameObject
	{
		public AudioSource velocityWindSource;

		public override void RenderUpdate()
		{
			/*if(!(Main.LocalEntity is LivingEntity localMob)) {
				return;
			}*/

			//var localEntities = Player.localPlayers.Select

			//test
			const float minWindSpeed = 9f;
			const float maxWindSpeed = 100f;
			float speed = Mathf.Sqrt(Player.localPlayers.Max(p => p.Entity?.velocity.SqrMagnitude ?? 0f));
			float goalVolume = 0f;

			if(speed>=minWindSpeed) {
				if(velocityWindSource==null) {
					(velocityWindSource = AddComponent<AudioSource>(c => {
						var clip = Resources.Get<AudioClip>("Sounds/Atmosphere/VelocityWind.ogg");
						c.Clip = clip;
						c.PlaybackOffset = Rand.Next(clip.LengthInSeconds);
						c.Volume = goalVolume;
						c.Is2D = true;
						c.Loop = true;
					})).Play();
				}

				goalVolume = Math.Min(1f,(speed-minWindSpeed)/(maxWindSpeed-minWindSpeed));
			}

			if(velocityWindSource!=null) {
				float newVolume = Mathf.StepTowards(velocityWindSource.Volume,goalVolume,Time.RenderDeltaTime*0.75f);
				if(newVolume==0f) {
					velocityWindSource.Dispose();
					velocityWindSource = null;
				}else {
					velocityWindSource.Volume = newVolume;
				}
			}

			//var pos = localEntity.Transform.Position;
			//var tagsVolume = new Dictionary<string,float>();

			//float birdsVolume = 1f-(Math.Abs(pos.y-32f)/32f);
			
		}
	}
}
