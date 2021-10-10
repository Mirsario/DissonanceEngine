using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dissonance.Engine.IO
{
	//TODO: Make a ton more threadsafe.
	public sealed class Resources : EngineModule
	{
		internal class AssetReaderCollection<T>
		{
			public List<IAssetReader<T>> AssetReaders { get; } = new();
			public Dictionary<string, IAssetReader<T>> AssetReaderByExtension { get; } = new();
		}

		internal static readonly HashSet<object> Readers = new();
		internal static readonly Dictionary<Type, object> ReaderCollectionsByType = new();

		private static readonly HashSet<AssetSource> sources = new();
		private static readonly Dictionary<string, Asset> assets = new();

		protected override void Init()
		{
			RegisterAssetSources();
			RegisterAssetReaders();
		}

		public static Asset<T> Get<T>(string assetPath, AssetRequestMode mode = AssetRequestMode.None)
		{
			if (assets.TryGetValue(assetPath, out var cachedAsset) && cachedAsset is Asset<T> cachedAssetResult) {
				return cachedAssetResult;
			}

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

		public static void AddAssetReader<T>(IAssetReader<T> assetReader)
		{
			if (!Readers.Add(assetReader)) {
				throw new InvalidOperationException($"Asset reader '{assetReader.GetType().Name}' is already registered.");
			}

			if (!ReaderCollectionsByType.TryGetValue(typeof(T), out object collectionObj) || collectionObj is not AssetReaderCollection<T> collection) {
				ReaderCollectionsByType[typeof(T)] = collection = new AssetReaderCollection<T>();
			}

			collection.AssetReaders.Add(assetReader);

			foreach (string extension in assetReader.Extensions) {
				collection.AssetReaderByExtension.Add(extension, assetReader);
			}
		}

		private static void RegisterAssetSources()
		{
			string assetsPath = Path.GetFullPath(Path.Combine(".", "Assets"));

			if (!Directory.Exists(assetsPath)) {
				throw new DirectoryNotFoundException($"Unable to locate the Assets folder. Is the working directory set correctly?\r\nExpected it to be '{Path.GetFullPath(assetsPath)}'.");
			}

			AddAssetSource(new FileSystemAssetSource(assetsPath));
			AddAssetSource(new AssemblyResourcesAssetSource(Assembly.GetExecutingAssembly(), "Dissonance/Engine"));
		}

		private static void RegisterAssetReaders()
		{
			AddAssetReader(new PngReader());
			AddAssetReader(new ShaderReader());
			AddAssetReader(new MaterialReader());
		}
	}
}
