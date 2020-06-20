using System;
using Dissonance.Engine.Core.Modules;
using Dissonance.Engine.Graphics;
using Dissonance.Framework.Audio;

namespace Dissonance.Engine
{
	[ModuleDependency(typeof(Windowing))]
	public sealed class Audio : EngineModule
	{
		internal static IntPtr audioDevice;
		internal static IntPtr audioContext;

		public override bool AutoLoad => !Game.NoAudio;

		protected override void Init()
		{
			try {
				audioDevice = ALC.OpenDevice(null);
				audioContext = ALC.CreateContext(audioDevice,null);
			}
			catch(Exception e) {
				throw new AudioException("An issue occured during Audio initialization.",e);
			}

			if(!ALC.MakeContextCurrent(audioContext)) {
				throw new AudioException("An issue occured during Audio initialization: Unable to make the audio context current.");
			}

			CheckALErrors();

			AL.DistanceModel(DistanceModel.LinearDistanceClamped);

			CheckALErrors();
		}
		protected override void FixedUpdate() => CheckALErrors();

		[Obsolete("This call is supposed to be temporary.")]
		public static void CheckALErrorsTemp() => CheckALErrors();
		public static void CheckALErrors()
		{
			var error = AL.GetError();

			if(error!=AudioError.NoError) {
				throw new Exception("AudioError: "+error);
			}
		}

		internal static void Dispose() {}
	}
}
