using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Dissonance.Engine.IO
{
	public sealed class EntityPrefabReader : IAssetReader<EntityPrefab>
	{
		private static readonly MethodInfo EntityPrefabSetMethod = typeof(EntityPrefab).GetMethod(nameof(EntityPrefab.Set), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

		public string[] Extensions { get; } = { ".prefab" };

		public bool AutoloadAssets => true;

		public async ValueTask<EntityPrefab> ReadFromStream(Stream stream, string assetPath, MainThreadCreationContext switchToMainThread)
		{
			var json = Assets.Get<JObject>(assetPath, AssetRequestMode.ImmediateLoad).Value;
			var entity = WorldManager.PrefabWorld.CreateEntity();
			var prefab = new EntityPrefab(entity.Id);

			ParseComponents(prefab, json, assetPath);

			string contentName = Path.GetFileNameWithoutExtension(assetPath);

			GameContent.Register(contentName, prefab);

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
