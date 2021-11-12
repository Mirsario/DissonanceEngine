using System.IO;
using System.Threading.Tasks;

namespace Dissonance.Engine.IO
{
	public sealed class BytesReader : IAssetReader<byte[]>
	{
		public string[] Extensions { get; } = { ".bytes" };

		public async ValueTask<byte[]> ReadAsset(AssetFileEntry assetFile, MainThreadCreationContext switchToMainThread)
		{
			using var stream = assetFile.OpenStream();

			byte[] bytes = new byte[stream.Length];

			stream.Read(bytes, 0, bytes.Length);

			return bytes;
		}
	}
}
