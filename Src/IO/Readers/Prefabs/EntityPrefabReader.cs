using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Dissonance.Engine.IO
{
	public sealed class EntityPrefabReader : IAssetReader<EntityPrefab>
	{
		private static readonly MethodInfo EntityPrefabSetMethod = typeof(EntityPrefab).GetMethod(nameof(EntityPrefab.Set), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		
		public static bool AutoloadAssets { get; set; }

		public string[] Extensions { get; } = { ".prefab" };

		bool IAssetReader<EntityPrefab>.AutoloadAssets => AutoloadAssets;

		public async ValueTask<EntityPrefab> ReadAsset(AssetFileEntry assetFile, MainThreadCreationContext switchToMainThread)
		{
			string assetPath = assetFile.Path;

			var json = Assets.Get<JObject>(assetPath, AssetRequestMode.ImmediateLoad).Value;
			var entity = WorldManager.PrefabWorld.CreateEntity();
			var prefab = new EntityPrefab(entity.Id);

			ParseComponents(prefab, json, assetPath);

			return prefab;
		}

		private static void ParseComponents(EntityPrefab prefab, JObject jsonContainer, string assetPath)
		{
			AssetJsonConverter.BaseAssetPath = Assets.FilterPath(Path.GetDirectoryName(assetPath));

			object[] parameterArray = new object[1];

			foreach (var pair in jsonContainer) {
				var jsonElement = pair.Value;
				var componentType = ComponentManager.GetComponentTypeFromName(pair.Key);

				parameterArray[0] = jsonElement.ToObject(componentType);

				//TODO: Optimize.
				EntityPrefabSetMethod
					.MakeGenericMethod(componentType)
					.Invoke(prefab, parameterArray);
			}
		}
	}
}
