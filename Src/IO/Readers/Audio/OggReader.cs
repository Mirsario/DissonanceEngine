using System.IO;
using System.Threading.Tasks;
using Dissonance.Engine.Audio;
using NVorbis;

namespace Dissonance.Engine.IO;

public class OggReader : IAssetReader<AudioClip>
{
	public string[] Extensions { get; } = { ".ogg" };

	public async ValueTask<AudioClip> ReadAsset(AssetFileEntry assetFile, MainThreadCreationContext switchToMainThread)
	{
		using var stream = assetFile.OpenStream();
		using var reader = new VorbisReader(stream, true);

		long bufferSize = reader.TotalSamples * reader.Channels;
		float[] data = new float[bufferSize];

		reader.ReadSamples(data, 0, (int)bufferSize);

		var clip = new AudioClip();

		clip.SetData(data, reader.Channels, sizeof(float), reader.SampleRate);

		return clip;
	}
}
