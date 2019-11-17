using System;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace GameEngine
{
	public static class Audio
	{
		//TODO: Currently,there can't be more than 1 listener in world. This is really bad for games with splitscreen.
		internal static AudioContext defaultContext;
		
		internal static void Init()
		{
			try {
				defaultContext = new AudioContext();
			}
			catch {
				throw new AudioException("An issue occured during Audio initialization.\r\nIs OpenAL installed?");
			}

			defaultContext.Suspend();

			CheckALErrors();
			
			AL.DistanceModel(ALDistanceModel.LinearDistanceClamped);
			
			CheckALErrors();
		}
		internal static void FixedUpdate() => CheckALErrors();
		internal static void Dispose() {}
		internal static void CheckALErrors()
		{
			var error = AL.GetError();

			if(error!=ALError.NoError) {
				throw new Exception("ALError: "+error);
			}
		}
	}
}
