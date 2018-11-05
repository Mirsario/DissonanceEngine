using System;
using System.IO;
using OpenTK.Audio.OpenAL;
using OpenTK.Audio;

namespace GameEngine
{
	[AllowOnlyOneInWorld]
	public class AudioListener : Component
	{
		public AudioContext audioContext;

		protected override void OnInit()
		{
			
		}
		protected override void OnEnable()
		{
			if(audioContext==null) {
				audioContext = Audio.defaultContext;
			}
			audioContext.Process();
		}
		protected override void OnDisable()
		{
			audioContext.Suspend();
		}
		public override void FixedUpdate()
		{
			OpenTK.Vector3 pos = Transform.Position;
			AL.Listener(ALListener3f.Position,ref pos);
			//AL.Listener(ALListenerf.Gain,1f);
			OpenTK.Vector3 lookAt = -Transform.Forward;
			OpenTK.Vector3 up = Transform.Up;
			AL.Listener(ALListenerfv.Orientation,ref lookAt,ref up);
		}
	}
}

