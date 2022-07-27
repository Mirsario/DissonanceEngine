using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Dissonance.Engine.IO;

public sealed class EntityPrefabReader : IAssetReader<EntityPrefab>
{
	private static readonly MethodInfo EntityPrefabSetMethod = typeof(EntityPrefab).GetMethod(nameof(EntityPrefab.Set), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
	
	public static bool AutoloadAssets { get; set; }

	public string[] Extensions { get; } = { ".prefab" };

	bool IAssetReader<EntityPrefab>.AutoloadAssets => AutoloadAssets;

	public async ValueTask<EntityPrefab> ReadAsset(AssetFileEntry assetFile, MainThreadCreationContext switchToMainThread)
	{
		string assetPath = assetFile.Path;

		await switchToMainThread; // Thread-safety is quite bad for now.

		// A better design would be nice.
		AssetJsonConverter.BaseAssetPath = Assets.FilterPath(Path.GetDirectoryName(assetPath));

		var json = Assets.Get<JObject>(assetPath, AssetRequestMode.ImmediateLoad).Value;
		var prefab = json.ToObject<EntityPrefab>();

		return prefab;
	}
}
