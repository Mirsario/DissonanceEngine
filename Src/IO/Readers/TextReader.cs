using System.IO;

namespace Dissonance.Engine.IO
{
	public class TextReader : IAssetReader<string>
	{
		public string[] Extensions { get; } = { "*", ".txt" };

		public string ReadFromStream(Stream stream, string assetPath)
		{
			using var reader = new StreamReader(stream);

			return reader.ReadToEnd();
		}
	}
}
