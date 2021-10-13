using System;
using System.Runtime.InteropServices;
using Dissonance.Framework.Audio;

namespace Dissonance.Engine.Audio
{
	public sealed class AudioClip : IDisposable
	{
		private int channelsNum;
		private int bytesPerSample;
		private int sampleRate;

		public float LengthInSeconds { get; private set; }

		internal uint BufferId { get; private set; }

		public AudioClip()
		{
			BufferId = AL.GenBuffer();
		}

		public void Dispose()
		{
			if (BufferId > 0) {
				AL.DeleteBuffer(BufferId);

				BufferId = 0;
			}
		}

		public unsafe void SetData<T>(T[] data, int channelsNum, int bytesPerSample, int sampleRate) where T : unmanaged
		{
			if (data == null) {
				throw new ArgumentNullException(nameof(data));
			}

			this.channelsNum = channelsNum;
			this.bytesPerSample = bytesPerSample;
			this.sampleRate = sampleRate;

			LengthInSeconds = data.Length / (float)sampleRate / channelsNum;

			var format = GetSoundFormat(channelsNum, bytesPerSample);

			fixed(T* ptr = data) {
				AL.BufferData(BufferId, format, (IntPtr)ptr, data.Length * Marshal.SizeOf<T>(), sampleRate);
			}

			AudioEngine.CheckALErrors();
		}

		public static BufferFormat GetSoundFormat(int channels, int bitsPerSample) => bitsPerSample switch
		{
			1 => channels == 1 ? BufferFormat.Mono8 : BufferFormat.Stereo8,
			2 => channels == 1 ? BufferFormat.Mono16 : BufferFormat.Stereo16,
			4 => channels == 1 ? (BufferFormat)0x10010 : (BufferFormat)0x10011,
			_ => throw new NotSupportedException("The specified sound format is not supported."),
		};
	}
}

