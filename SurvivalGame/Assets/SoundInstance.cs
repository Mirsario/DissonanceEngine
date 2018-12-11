using System;
using System.Collections.Generic;
using GameEngine;

namespace SurvivalGame
{
	public class SoundInstance : GameObject
	{
		public AudioSource source;
		//private string sound;

		public override void FixedUpdate()
		{
			if(source==null || !source.IsPlaying) {
				Dispose();
			}
		}

		public static SoundInstance Create(string sound,Vector3 position,float volume = 1f,Transform attachTo = null)
		{
			var instance = Instantiate<SoundInstance>("SoundInstance_"+sound);
			if(attachTo!=null) {
				instance.Transform.parent = attachTo;
			}
			instance.Transform.Position = position;
			instance.source = instance.AddComponent<AudioSource>();
			instance.source.Clip = Resources.Get<AudioClip>(sound);
			instance.source.Volume = volume;
			instance.source.Play();
			return instance;
		}
	}
}