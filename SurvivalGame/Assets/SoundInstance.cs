using System;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	public class SoundInstance : GameObject
	{
		public AudioSource source;
		//private string sound;

		public SoundInstance(string sound,Vector3 position,float volume = 1f,Transform attachTo = null) : base("SoundInstance_"+sound)
		{
			//this.sound = sound;
			if(attachTo!=null) {
				Transform.parent = attachTo;
			}
			Transform.Position = position;
			source = AddComponent<AudioSource>();
			source.Clip = Resources.Get<AudioClip>(sound);
			source.Volume = volume;
			source.Play();
		}
		public override void FixedUpdate()
		{
			if(source==null || !source.IsPlaying) {
				Dispose();
			}
		}
	}
}