using System;
using Dissonance.Engine.Graphics;
using static Dissonance.Engine.Audio.OpenALApi;
using ALDevice = Silk.NET.OpenAL.Device;
using ALContext = Silk.NET.OpenAL.Context;
using Silk.NET.OpenAL;

namespace Dissonance.Engine.Audio
{
	[ModuleDependency<OpenALApi>]
	[ModuleDependency<Windowing>(isOptional: true)]
	[Autoload(DisablingGameFlags = GameFlags.NoAudio)]
	public unsafe sealed class AudioEngine : EngineModule
	{
		internal static ALDevice* audioDevice;
		internal static ALContext* audioContext;

		protected override void Init()
		{
			try {
				audioDevice = OpenALContext.OpenDevice(null);
				audioContext = OpenALContext.CreateContext(audioDevice, null);
			}
			catch (Exception e) {
				throw new AudioException($"An issue occured during Audio initialization: {e}");
			}

			if (!OpenALContext.MakeContextCurrent(audioContext)) {
				throw new AudioException("An issue occured during Audio initialization: Unable to make the audio context current.");
			}

			if (!OpenAL.IsExtensionPresent("AL_EXT_float32")) {
				throw new AudioException("No float32 audio support extension found!");
			}

			CheckALErrors();

			OpenAL.DistanceModel(DistanceModel.LinearDistanceClamped);

			CheckALErrors();
		}

		protected override void FixedUpdate() => CheckALErrors();

		[Obsolete("This call is supposed to be temporary.")]
		public static void CheckALErrorsTemp() => CheckALErrors();

		public static void CheckALErrors()
		{
			var error = OpenAL.GetError();

			if (error != AudioError.NoError) {
				throw new Exception("AudioError: " + error);
			}
		}
	}
}
