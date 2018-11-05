using System;
using System.IO;
using OpenTK.Audio.OpenAL;

namespace GameEngine
{
	public class AudioClip : Asset<AudioClip>
	{
		//TODO: Implement OnDispose

		internal int bufferId = -1;

		internal int channelsNum;
		internal int bitsPerSample;
		internal int sampleRate;
		
		internal AudioClip()
		{
			Audio.CheckALErrors();
			bufferId = AL.GenBuffer();
			Audio.CheckALErrors();
		}
		internal void SetData(byte[] data,int channelsNum,int bitsPerSample,int sampleRate)
		{
			if(data==null) {
				throw new ArgumentNullException("data");
			}
			this.channelsNum = channelsNum;
			this.bitsPerSample = bitsPerSample;
			this.sampleRate = sampleRate;
			Audio.CheckALErrors();
			AL.BufferData(bufferId,GetSoundFormat(channelsNum,bitsPerSample),data,data.Length,sampleRate);
			Audio.CheckALErrors();
		}
		internal void SetData(short[] data,int channelsNum,int bytesPerSample,int sampleRate)
		{
			if(data==null) {
				throw new ArgumentNullException("data");
			}
			this.channelsNum = channelsNum;
			this.sampleRate = sampleRate;
			Audio.CheckALErrors();
			AL.BufferData(bufferId,ALFormat.Mono16,data,data.Length*bytesPerSample,sampleRate);
			Audio.CheckALErrors();
		}
		internal void SetData(float[] data,int channelsNum,int bytesPerSample,int sampleRate)
		{
			if(data==null) {
				throw new ArgumentNullException("data");
			}
			this.channelsNum = channelsNum;
			this.sampleRate = sampleRate;
			Audio.CheckALErrors();
			AL.BufferData(bufferId,ALFormat.Mono16,data,data.Length*bytesPerSample,sampleRate);
			Audio.CheckALErrors();
		}

		internal static ALFormat GetSoundFormat(int channels,int bits)
		{
			switch(channels) {
				case 1:		return bits==8 ? ALFormat.Mono8 : ALFormat.Mono16;
				case 2:		return bits==8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
				default:	throw new NotSupportedException("The specified sound format is not supported.");
			}
		}
	}
}

