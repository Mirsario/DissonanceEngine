using System.IO;
using NVorbis;

namespace GameEngine
{
	public class OggManager : AssetManager<AudioClip>
	{
		public override string[] Extensions => new [] { ".ogg" };
		
		public override AudioClip Import(Stream stream,string fileName)
		{
			using var r = new VorbisReader(stream,true);

			long bufferSize = r.TotalSamples*r.Channels;
			var data = new float[bufferSize];

			r.ReadSamples(data,0,(int)bufferSize);

			var clip = new AudioClip();

			clip.SetData(data,r.Channels,sizeof(float),r.SampleRate);

			return clip;
		}
	}
}
