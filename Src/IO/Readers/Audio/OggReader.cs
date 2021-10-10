using System.IO;
using Dissonance.Engine.Audio;
using NVorbis;

namespace Dissonance.Engine.IO
{
	public class OggReader : IAssetReader<AudioClip>
	{
		public string[] Extensions { get; } = { ".ogg" };

		public AudioClip ReadFromStream(Stream stream, string assetPath)
		{
			using var r = new VorbisReader(stream, true);

			long bufferSize = r.TotalSamples * r.Channels;
			float[] data = new float[bufferSize];

			r.ReadSamples(data, 0, (int)bufferSize);

			var clip = new AudioClip();

			clip.SetData(data, r.Channels, sizeof(float), r.SampleRate);

			return clip;
		}
	}
}
