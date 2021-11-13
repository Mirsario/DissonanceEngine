using System.IO;
using System.Threading.Tasks;

namespace Dissonance.Engine.IO
{
	public class TextReader : IAssetReader<string>
	{
		public string[] Extensions { get; } = { "*", ".txt" };

		public async ValueTask<string> ReadAsset(AssetFileEntry assetFile, MainThreadCreationContext switchToMainThread)
		{
			using var stream = assetFile.OpenStream();
			using var reader = new StreamReader(stream);

			return reader.ReadToEnd();
		}
	}
}
