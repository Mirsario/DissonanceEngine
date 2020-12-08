using System.IO;
using Dissonance.Engine.Audio;
using NVorbis;

namespace Dissonance.Engine.IO
{
	public class OggManager : AssetManager<AudioClip>
	{
		public override string[] Extensions => new[] { ".ogg" };

		public override AudioClip Import(Stream stream, string filePath)
		{
			using var r = new VorbisReader(stream, true);

			long bufferSize = r.TotalSamples * r.Channels;
			var data = new float[bufferSize];

			r.ReadSamples(data, 0, (int)bufferSize);

			var clip = new AudioClip();

			clip.SetData(data, r.Channels, sizeof(float), r.SampleRate);

			return clip;
		}
	}
}
