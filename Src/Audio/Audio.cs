using System;
using Dissonance.Framework;
using Dissonance.Framework.OpenAL;

namespace Dissonance.Engine
{
	public static class Audio
	{
		internal static IntPtr audioDevice;
		internal static IntPtr audioContext;

		[Obsolete("This call is supposed to be temporary.")]
		public static void CheckALErrorsTemp() => CheckALErrors();
		public static void CheckALErrors()
		{
			var error = AL.GetError();

			if(error!=AudioError.NoError) {
				throw new Exception("AudioError: "+error);
			}
		}

		internal static void Init()
		{
			try {
				audioDevice = ALC.OpenDevice(null);
				audioContext = ALC.CreateContext(audioDevice,new int[] { });
			}
			catch(Exception e) {
				throw new AudioException("An issue occured during Audio initialization.",e);
			}

			if(!ALC.MakeContextCurrent(audioContext)) {
				throw new AudioException("An issue occured during Audio initialization: Unable to make the audio context current.");
			}

			ALC.SuspendContext(audioContext);

			CheckALErrors();
			
			AL.DistanceModel(DistanceModel.LinearDistanceClamped);

			Debug.Log("Supports float32 audio: "+AL.IsExtensionPresent("AL_EXT_FLOAT32"));
			
			CheckALErrors();
		}
		internal static void FixedUpdate() => CheckALErrors();
		internal static void Dispose() {}
	}
}
