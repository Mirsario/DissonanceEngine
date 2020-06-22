using System;
using System.Runtime.InteropServices;
using Dissonance.Engine.IO;
using Dissonance.Framework.Audio;

namespace Dissonance.Engine.Audio
{
	public class AudioClip : Asset
	{
		internal uint bufferId;

		protected int channelsNum;
		protected int bytesPerSample;
		protected int sampleRate;

		public float LengthInSeconds { get; private set; }

		public AudioClip()
		{
			bufferId = AL.GenBuffer();
		}

		public override void Dispose()
		{
			if(bufferId>0) {
				AL.DeleteBuffer(bufferId);

				bufferId = 0;
			}
		}

		public unsafe void SetData<T>(T[] data,int channelsNum,int bytesPerSample,int sampleRate) where T : unmanaged
		{
			if(data==null) {
				throw new ArgumentNullException(nameof(data));
			}

			this.channelsNum = channelsNum;
			this.bytesPerSample = bytesPerSample;
			this.sampleRate = sampleRate;

			LengthInSeconds = data.Length/(float)sampleRate/channelsNum;

			var format = GetSoundFormat(channelsNum,bytesPerSample);

			fixed(T* ptr = data) {
				AL.BufferData(bufferId,format,(IntPtr)ptr,data.Length*Marshal.SizeOf<T>(),sampleRate);
			}

			AudioEngine.CheckALErrors();
		}

		public static BufferFormat GetSoundFormat(int channels,int bitsPerSample) => bitsPerSample switch
		{
			1 => channels==1 ? BufferFormat.Mono8 : BufferFormat.Stereo8,
			2 => channels==1 ? BufferFormat.Mono16 : BufferFormat.Stereo16,
			4 => channels==1 ? (BufferFormat)0x10010 : (BufferFormat)0x10011,
			_ => throw new NotSupportedException("The specified sound format is not supported."),
		};
	}
}

