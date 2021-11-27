using System;
using Dissonance.Engine.Graphics;
using Dissonance.Framework.Audio;

namespace Dissonance.Engine.Audio
{
	[ModuleDependency(true, typeof(Windowing))]
	[ModuleAutoload(DisablingGameFlags = GameFlags.NoAudio)]
	public sealed class AudioEngine : EngineModule
	{
		internal static IntPtr audioDevice;
		internal static IntPtr audioContext;

		protected override void Init()
		{
			try {
				audioDevice = ALC.OpenDevice(null);
				audioContext = ALC.CreateContext(audioDevice, null);
			}
			catch (Exception e) {
				throw new AudioException("An issue occured during Audio initialization.", e);
			}

			if (!ALC.MakeContextCurrent(audioContext)) {
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

			if (error != AudioError.NoError) {
				throw new Exception("AudioError: " + error);
			}
		}
	}
}
