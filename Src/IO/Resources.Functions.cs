using System;
using System.Reflection;
using System.IO;
using System.Linq;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine.IO
{
	partial class Resources
	{
		// 'Import' simply imports files, optionally caching them for Get.
		public static T Import<T>(string filePath, bool addToCache = true, AssetManager<T> assetManager = null)
		{
			NameToPath(ref filePath, out bool ntpMultiplePaths);

			return ImportInternal(filePath, addToCache, assetManager, ntpMultiplePaths);
		}

		public static T ImportFromStream<T>(Stream stream, AssetManager<T> assetManager = null, string filePath = null)
		{
			var type = typeof(T);

			if (assetManager == null) {
				if (filePath == null) {
					throw new ArgumentException(
						$"Could not figure out an AssetManager for import, since '{nameof(filePath)}' parameter is null." +
						$"\r\nProvide a proper file name, or provide an AssetManager with the '{nameof(assetManager)}' parameter."
					);
				}

				string ext = Path.GetExtension(filePath).ToLower();

				if (!assetManagers.TryGetValue(ext, out var managers)) {
					throw new NotImplementedException($"Could not find any asset managers for the '{ext}' extension.");
				}

				var results = managers.SelectIgnoreNull(q => q as AssetManager<T>).ToArray();

				if (results.Length == 0) {
					throw new NotImplementedException($"Could not find any '{ext}' asset managers which would return a '{type.Name}'.");
				}

				assetManager = results[0];
			}

			var output = assetManager.Import(stream, filePath);

			InternalUtils.ObjectOrCollectionCall<Asset>(output, asset => asset.RegisterAsset(), false);

			return output;
		}

		public static string ImportText(string filePath, bool addToCache = false)
			=> Import(filePath, addToCache, (AssetManager<string>)assetManagers[".txt"][0]);

		public static byte[] ImportBytes(string filePath, bool addToCache = false)
			=> Import(filePath, addToCache, (AssetManager<byte[]>)assetManagers[".bytes"][0]);

		// 'Get' imports and caches files, or gets them from cache, if they have already been loaded.
		public static T Get<T>(string filePath)
		{
			ReadyPath(ref filePath);
			NameToPath(ref filePath, out bool ntpMultiplePaths);

			if (cacheByPath.TryGetValue(typeof(T), out var dict) && dict.TryGetValue(filePath, out var obj) && obj is T content) {
				return content;
			}

			return ImportInternal<T>(filePath, true, null, ntpMultiplePaths);
		}

		// 'Find' finds already loaded resources by their internal asset names, if they have them. Exists mostly for stuff like shaders.
		public static bool Find<T>(string assetName, out T asset) where T : class
			=> (asset = Find<T>(assetName)) != null;

		public static T Find<T>(string assetName, bool throwOnFail = true) where T : class
		{
			var type = typeof(T);

			if (cacheByName.TryGetValue(type, out var realDict) && realDict.TryGetValue(assetName, out var obj) && obj is T content) {
				return content;
			}

			if (throwOnFail) {
				throw new Exception($"Couldn't find '{typeof(T).Name}' asset with name '{assetName}'.");
			}

			return null;
		}

		// 'Export' exports assets
		public static void Export<T>(T asset, string filePath, AssetManager<T> assetManager = null) where T : class
		{
			var type = typeof(T);

			ReadyPath(ref filePath);

			if (assetManager == null) {
				string ext = Path.GetExtension(filePath).ToLower();

				if (!assetManagers.TryGetValue(ext, out var managers)) {
					throw new NotImplementedException($"Could not find any asset managers for the '{ext}' extension.");
				}

				var results = managers.SelectIgnoreNull(q => q as AssetManager<T>).ToArray();

				if (results.Length != 1) {
					if (results.Length == 0) {
						throw new NotImplementedException($"Could not find any '{ext}' asset managers which would return a {type.Name}.");
					}

					throw new NotImplementedException(
						$"Found more than 1 '{ext}' asset managers which would return a {type.Name}:\r\n{string.Join(",\r\n", results.Select(q => q.GetType().Name))}\r\n\r\nPlease specify which asset manager should be used via the '{nameof(assetManager)}' parameter."
					);
				}

				assetManager = results[0];
			}

			using var stream = File.OpenWrite(filePath);

			assetManager.Export(asset, stream);
		}

		internal static T ImportInternal<T>(string filePath, bool addToCache, AssetManager<T> assetManager, bool ntpMultiplePaths)
		{
			ReadyPath(ref filePath);

			if (importingBuiltInAssets || filePath.ToLower().StartsWith(BuiltInAssetsFolder.ToLower())) {
				return (T)ImportBuiltInAsset(filePath, assetManager);
			}

			if (!File.Exists(filePath)) {
				if (ntpMultiplePaths) {
					throw new FileNotFoundException($"Couldn't find file '{filePath}' for import. There were multiple path aliases for that filename.");
				}

				throw new FileNotFoundException($"Couldn't find file '{filePath}' for import.");
			}

			using var stream = File.OpenRead(filePath);
			var content = ImportFromStream(stream, assetManager, filePath);

			if (addToCache) {
				AddToCache(filePath, content);
			}

			return content;
		}

		internal static object ImportBuiltInAsset(string filePath, AssetManager manager = null, byte[] data = null)
		{
			ReadyPath(ref filePath);

			if (data == null && !builtInAssets.TryGetValue(filePath, out data)) {
				throw new FileNotFoundException($"Couldn't find built-in asset at '{filePath}'.");
			}

			if (manager == null) {
				if (!assetManagers.TryGetValue(Path.GetExtension(filePath).ToLower(), out var list)) {
					return null;
				}

				manager = list[0];
			}

			using var entryStream = new MemoryStream(data);

			var tType = manager.GetType().BaseType?.GetGenericArguments()[0];

			if (tType == null) {
				return null;
			}

			//TODO: Avoid these generic shenanigans.

			var method = typeof(Resources).GetMethod(nameof(ImportFromStream), BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(tType);
			object content = method.Invoke(manager, new object[] { entryStream, manager, filePath });

			AddToCache(filePath, content);

			return content;
		}
	}
}
