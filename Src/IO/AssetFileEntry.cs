using System.IO;

namespace Dissonance.Engine.IO;

public class AssetFileEntry
{
	public readonly string Path;
	public readonly IAssetSource Source;

	public AssetFileEntry(string path, IAssetSource source)
	{
		Path = path;
		Source = source;
	}

	public Stream OpenStream()
	{
		return Source.OpenStream(Path);
	}
}
