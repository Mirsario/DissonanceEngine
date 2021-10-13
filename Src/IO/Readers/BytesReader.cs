using System.IO;
using System.Threading.Tasks;

namespace Dissonance.Engine.IO
{
	public sealed class BytesReader : IAssetReader<byte[]>
	{
		public string[] Extensions { get; } = { ".bytes" };

		public async ValueTask<byte[]> ReadFromStream(Stream stream, string assetPath, MainThreadCreationContext switchToMainThread)
		{
			byte[] bytes = new byte[stream.Length];

			stream.Read(bytes, 0, bytes.Length);

			return bytes;
		}
	}
}
