using System.IO;
using System.Threading.Tasks;

namespace Dissonance.Engine.IO
{
	public class TextReader : IAssetReader<string>
	{
		public string[] Extensions { get; } = { "*", ".txt" };

		public async ValueTask<string> ReadFromStream(Stream stream, string assetPath, MainThreadCreationContext switchToMainThread)
		{
			using var reader = new StreamReader(stream);

			return reader.ReadToEnd();
		}
	}
}
