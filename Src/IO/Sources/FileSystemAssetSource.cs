using System.Collections.Generic;
using System.IO;

namespace Dissonance.Engine.IO;

public sealed class FileSystemAssetSource : IAssetSource
{
	private readonly string basePath;
	private readonly Dictionary<string, string> assetPathToFullPath = new();

	public FileSystemAssetSource(string basePath)
	{
		this.basePath = basePath;

		RecalculatePathsCache();
	}
	
	public IEnumerable<string> EnumerateAssets()
		=> assetPathToFullPath.Keys;

	public bool HasAsset(string path)
		=> assetPathToFullPath.ContainsKey(path);

	public Stream OpenStream(string path)
		=> File.OpenRead(assetPathToFullPath[path]);

	public void RecalculatePathsCache()
	{
		foreach (var file in new DirectoryInfo(basePath).EnumerateFiles("*", SearchOption.AllDirectories)) {
			string filePath = file.FullName;
			string relativeFilePath = Path.GetRelativePath(basePath, filePath);

			relativeFilePath = relativeFilePath.Replace('\\', '/');

			assetPathToFullPath[relativeFilePath] = filePath;
		}
	}
}
