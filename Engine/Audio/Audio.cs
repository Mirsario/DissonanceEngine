using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
			defaultContext = new AudioContext();
			defaultContext.Suspend();

			CheckALErrors();
			//Debug.Log("Default distance model: "+AL.GetDistanceModel());
			AL.DistanceModel(ALDistanceModel.InverseDistance);
			//AL.DopplerFactor(1f);
			CheckALErrors();
			//int source = AL.GenSource();//audiosource
			//int state;

			//AL.Source(source,ALSourcei.Buffer,buffer);	//Binds audio clip to an audio source
			//AL.SourcePlay(source);

			//Query the source to find out when it stops playing.
			/*do {
				Thread.Sleep(250);
				AL.GetSource(source,ALGetSourcei.SourceState,out state);
			}
			while ((ALSourceState)state==ALSourceState.Playing);*/

			/*AL.SourceStop(source);
			AL.DeleteSource(source);
			AL.DeleteBuffer(buffer);*/
		}
		internal static void FixedUpdate()
		{
			CheckALErrors();
		}
		internal static void Dispose()
		{
			
		}

		public static void CheckALErrors()
		{
			var error = AL.GetError();
			if(error!=ALError.NoError) {
				throw new Exception("ALError: "+error.ToString());
			}
		}
	}
}
