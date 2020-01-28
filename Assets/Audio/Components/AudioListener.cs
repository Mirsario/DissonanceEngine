using Dissonance.Framework.OpenAL;
using System;

namespace GameEngine
{
	[AllowOnlyOneInWorld]
	public class AudioListener : Component
	{
		public IntPtr audioContext;

		private float[] orientationCache = new float[6];

		protected override void OnEnable()
		{
			if(audioContext==null) {
				audioContext = Audio.audioContext;
			}

			ALC.ProcessContext(audioContext);
		}
		protected override void OnDisable()
		{
			ALC.SuspendContext(audioContext);
		}
		public override void FixedUpdate()
		{
			Vector3 pos = Transform.Position;

			AL.Listener3(ListenerVector3Float.Position,pos.x,pos.y,pos.z);

			Vector3 lookAt = -Transform.Forward;
			Vector3 up = Transform.Up;

			orientationCache[0] = lookAt.x;
			orientationCache[1] = lookAt.y;
			orientationCache[2] = lookAt.z;
			orientationCache[3] = up.x;
			orientationCache[4] = up.y;
			orientationCache[5] = up.z;

			AL.Listener(ListenerFloatArray.Orientation,orientationCache);
		}
	}
}

