using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Dissonance.Engine.IO;

public sealed class AssemblyResourcesAssetSource : IAssetSource
{
	private readonly string basePath;
	private readonly Assembly assembly;
	private readonly Dictionary<string, string> assetPathToManifestPath = new();

	public AssemblyResourcesAssetSource(Assembly assembly, string basePath = null)
	{
		this.assembly = assembly;
		this.basePath = basePath;

		RecalculatePathsCache();
	}

	public IEnumerable<string> EnumerateAssets()
		=> assetPathToManifestPath.Keys;

	public bool HasAsset(string path)
		=> assetPathToManifestPath.ContainsKey(path);

	public Stream OpenStream(string path)
		=> assembly.GetManifestResourceStream(assetPathToManifestPath[path]);

	private void RecalculatePathsCache()
	{
		foreach (string manifestPath in assembly.GetManifestResourceNames()) {
			string extension = Path.GetExtension(manifestPath);
			string pathWithoutExtenion = Path.ChangeExtension(manifestPath, null);
			string assetPath = pathWithoutExtenion.Replace('.', '/') + extension;

			if (basePath != null) {
				assetPath = Path.GetRelativePath(basePath, assetPath);
			}

			assetPath = assetPath.Replace('\\', '/');

			assetPathToManifestPath[assetPath] = manifestPath;
		}
	}
}
