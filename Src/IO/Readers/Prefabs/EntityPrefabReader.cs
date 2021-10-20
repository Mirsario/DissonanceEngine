using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Dissonance.Engine.IO
{
	public sealed class EntityPrefabReader : IAssetReader<PackedEntity>
	{
		public string[] Extensions { get; } = { ".entity" };

		public async ValueTask<PackedEntity> ReadFromStream(Stream stream, string assetPath, MainThreadCreationContext switchToMainThread)
		{
			var json = Assets.Get<JObject>(assetPath, AssetRequestMode.ImmediateLoad).Value;
			var packedEntity = new PackedEntity();

			ParseComponents(packedEntity, json, assetPath);

			return packedEntity;
		}

		private static void ParseComponents(PackedEntity packedEntity, JObject jsonContainer, string assetPath)
		{
			AssetJsonConverter.BaseAssetPath = Assets.FilterPath(Path.GetDirectoryName(assetPath));

			foreach (var pair in jsonContainer) {
				var jsonElement = pair.Value;

				if (jsonElement.Type != JTokenType.Object) {
					continue;
				}

				var componentType = ComponentManager.GetComponentTypeFromName(pair.Key);

				packedEntity.SetComponent(componentType, jsonElement.ToObject(componentType));
			}
		}
	}
}
