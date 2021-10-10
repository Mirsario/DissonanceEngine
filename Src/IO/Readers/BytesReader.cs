using System.IO;

namespace Dissonance.Engine.IO
{
	public sealed class BytesReader : IAssetReader<byte[]>
	{
		public string[] Extensions { get; } = { ".bytes" };

		public byte[] ReadFromStream(Stream stream, string assetPath)
		{
			byte[] bytes = new byte[stream.Length];

			stream.Read(bytes, 0, bytes.Length);

			return bytes;
		}
	}
}
