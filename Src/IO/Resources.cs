using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Dissonance.Engine.IO
{
	//TODO: Make a ton more threadsafe.
	public sealed class Resources : EngineModule
	{
		private class AssetReaderCollection<T>
		{
			public List<IAssetReader<T>> AssetReaders { get; } = new();
			public Dictionary<string, IAssetReader<T>> AssetReaderByExtension { get; } = new();
		}

		private static readonly HashSet<AssetSource> sources = new();
		private static readonly HashSet<object> readers = new();
		private static readonly Dictionary<Type, object> readerCollectionsByType = new();
		private static readonly Dictionary<string, Asset> assets = new();

		protected override void Init()
		{
			string assetsPath = Path.GetFullPath(Path.Combine(".", "Assets"));

			if (!Directory.Exists(assetsPath)) {
				throw new DirectoryNotFoundException($"Unable to locate the Assets folder. Is the working directory set correctly?\r\nExpected it to be '{Path.GetFullPath(assetsPath)}'.");
			}

			AddAssetSource(new FileSystemAssetSource(assetsPath));
		}

		public static Asset<T> Get<T>(string assetPath, AssetRequestMode mode = AssetRequestMode.None)
		{
			if (assets.TryGetValue(assetPath, out var cachedAsset) && cachedAsset is Asset<T> cachedAssetResult) {
				return cachedAssetResult;
			}

			/*
			string extension = Path.GetExtension(assetPath);

			if (!assetReaderCollectionsByType.TryGetValue(typeof(T), out object result) || result is not AssetReaderCollection<T> assetReaderCollection) {
				throw new InvalidOperationException($"No asset reader found with a return type of '{typeof(T).Name}'.");
			}

			if (!assetReaderCollection.AssetReaderByExtension.TryGetValue(extension, out var assetReader)) {
				throw new InvalidOperationException($"No asset reader found for file extension '{extension}'.");
			}
			*/

			foreach (var source in sources) {
				if (!source.HasAsset(assetPath)) {
					continue;
				}

				var asset = new Asset<T>(assetPath, source);

				assets[assetPath] = asset;

				asset.Request(mode);

				return asset;
			}

			throw new Exception($"Couldn't find asset '{assetPath}' in any available content sources.");
		}

		public static Asset<T> Find<T>(string fullName)
			=> AssetLookup<T>.Get(fullName);

		public static bool TryFind<T>(string fullName, out Asset<T> result)
			=> AssetLookup<T>.TryGetValue(fullName, out result);

		public static void AddAssetSource(AssetSource assetSource)
		{
			sources.Add(assetSource);
		}
	}
}
