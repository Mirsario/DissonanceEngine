using System;
using OpenTK.Audio.OpenAL;

namespace GameEngine
{
	public class AudioClip : Asset<AudioClip>
	{
		internal int bufferId;

		internal int channelsNum;
		internal int bitsPerSample;
		internal int sampleRate;

		public float LengthInSeconds { get; private set; }
		
		internal AudioClip()
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

		internal void SetData(byte[] data,int channelsNum,int bitsPerSample,int sampleRate)
		{
			if(data==null) {
				throw new ArgumentNullException(nameof(data));
			}

			this.channelsNum = channelsNum;
			this.bitsPerSample = bitsPerSample;
			this.sampleRate = sampleRate;

			LengthInSeconds = data.Length/(float)sampleRate/channelsNum;

			AL.BufferData(bufferId,GetSoundFormat(channelsNum,bitsPerSample),data,data.Length,sampleRate);

			Audio.CheckALErrors();
		}
		internal void SetData(short[] data,int channelsNum,int bytesPerSample,int sampleRate)
		{
			if(data==null) {
				throw new ArgumentNullException(nameof(data));
			}

			this.channelsNum = channelsNum;
			this.sampleRate = sampleRate;

			LengthInSeconds = data.Length/(float)sampleRate/channelsNum;

			AL.BufferData(bufferId,ALFormat.Mono16,data,data.Length*bytesPerSample,sampleRate);

			Audio.CheckALErrors();
		}
		internal void SetData(float[] data,int channelsNum,int bytesPerSample,int sampleRate)
		{
			if(data==null) {
				throw new ArgumentNullException(nameof(data));
			}

			this.channelsNum = channelsNum;
			this.sampleRate = sampleRate;

			LengthInSeconds = data.Length/(float)sampleRate/channelsNum;

			AL.BufferData(bufferId,ALFormat.Mono16,data,data.Length*bytesPerSample,sampleRate);

			Audio.CheckALErrors();
		}

		private static ALFormat GetSoundFormat(int channels,int bits) => channels switch {
			1 => bits==8 ? ALFormat.Mono8 : ALFormat.Mono16,
			2 => bits==8 ? ALFormat.Stereo8 : ALFormat.Stereo16,
			_ => throw new NotSupportedException("The specified sound format is not supported."),
		};
	}
}

