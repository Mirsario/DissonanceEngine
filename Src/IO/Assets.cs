using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine.IO
{
	//TODO: Make a ton more threadsafe.
	public sealed class Assets : EngineModule
	{
		internal static class ReadersByDataType<T>
		{
			internal static readonly Dictionary<string, IAssetReader<T>> ReaderByExtension = new();
		}

		internal static readonly HashSet<IAssetReader<object>> Readers = new();
		internal static readonly Dictionary<Type, IReadOnlyList<IAssetReader<object>>> ReadersByType = new();

		private static readonly HashSet<AssetSource> sources = new();
		private static readonly Dictionary<string, Asset> assets = new();

		protected override void Init()
		{
			RegisterAssetSources();
			RegisterAssetReaders();
			AutoloadAssets();
		}

		/// <summary>
		/// Attempts to get or create an asset with the provided case-sensitive virtual asset path, optionally requesting it to be loaded with the provided mode. 
		/// </summary>
		/// <typeparam name="T"> The type of the asset. </typeparam>
		/// <param name="assetPath"> The path of the asset. This path is virtual and case-sensitive - system paths will not work. </param>
		/// <param name="mode"> The mode to request the asset's loading with. Defaults to <see cref="AssetRequestMode.DoNotLoad"/>. </param>
		/// <returns> <see cref="Asset"/>&lt;<typeparamref name="T"/>&gt; - an asset handle. </returns>
		/// <exception cref="ArgumentOutOfRangeException"> Asset couldn't be found. </exception>
		public static Asset<T> Get<T>(string assetPath, AssetRequestMode mode = AssetRequestMode.DoNotLoad)
		{
			if (assets.TryGetValue(assetPath, out var cachedAsset) && cachedAsset is Asset<T> cachedAssetResult) {
				if (mode != AssetRequestMode.DoNotLoad && cachedAssetResult.State == AssetState.NotLoaded) {
					cachedAssetResult.Request(mode);
				}

				return cachedAssetResult;
			}

			foreach (var source in sources) {
				if (!source.HasAsset(assetPath)) {
					continue;
				}

				return RequestFromSource<T>(source, assetPath, mode);
			}

			throw new ArgumentOutOfRangeException($"Couldn't find asset '{assetPath}' in any available content sources.");
		}

		/// <summary>
		/// Attempts to find a registered asset using its case-sensitive name instead of a path.
		/// <br/> Throws exceptions on failure.
		/// </summary>
		/// <typeparam name="T"> The type of the asset. </typeparam>
		/// <param name="assetName"> The case-sensitive name of the asset. This is not the same as its path. </param>
		/// <returns> <see cref="Asset"/>&lt;<typeparamref name="T"/>&gt; - an asset handle. </returns>
		/// <exception cref="KeyNotFoundException"> No registered asset could be found with the provided name. </exception>
		public static Asset<T> Find<T>(string assetName)
			=> AssetLookup<T>.Get(assetName);

		/// <summary>
		/// Safely attempts to find a registered asset using its case-sensitive name instead of a path.
		/// </summary>
		/// <typeparam name="T"> The type of the asset. </typeparam>
		/// <param name="assetName"> The case-sensitive name of the asset. This is not the same as its path. </param>
		/// <param name="result"> The resulting <see cref="Asset"/>&lt;<typeparamref name="T"/>&gt; - an asset handle., if it was found. </param>
		/// <returns> A boolean indicating whether the operation succeeded. </returns>
		public static bool TryFind<T>(string assetName, out Asset<T> result)
			=> AssetLookup<T>.TryGetValue(assetName, out result);

		/// <summary>
		/// Creates and returns a new untracked asset object with the provided name and value.
		/// <br/> Untracked assets cannot be reloaded and are loaded by default.
		/// </summary>
		/// <typeparam name="T"> The type of the asset. </typeparam>
		/// <param name="name"> The name that the created asset should have. </param>
		/// <param name="value"> The value that the created asset should have. Must not be null. </param>
		public static Asset<T> CreateUntracked<T>(string name, T value)
		{
			if (value is null) {
				throw new ArgumentNullException(nameof(value));
			}

			return new Asset<T>(name, null) {
				Value = value,
				State = AssetState.Loaded,
			};
		}

		public static void AddAssetSource(AssetSource assetSource)
		{
			sources.Add(assetSource);
		}

		public static void AddAssetReader<T>(IAssetReader<T> assetReader)
		{
			if (!Readers.Add((IAssetReader<object>)assetReader)) {
				throw new InvalidOperationException($"Asset reader '{assetReader.GetType().Name}' is already registered.");
			}

			List<IAssetReader<T>> readersOfThisType;

			if (ReadersByType.TryGetValue(typeof(T), out var readersOfThisTypeTemp)) {
				readersOfThisType = (List<IAssetReader<T>>)readersOfThisTypeTemp;
			} else {
				readersOfThisType = new List<IAssetReader<T>>();
				ReadersByType[typeof(T)] = (IReadOnlyList<IAssetReader<object>>)readersOfThisType;
			}

			readersOfThisType.Add(assetReader);

			foreach (string extension in assetReader.Extensions) {
				ReadersByDataType<T>.ReaderByExtension.Add(extension, assetReader);
			}
		}

		private static Asset<T> RequestFromSource<T>(AssetSource source, string assetPath, AssetRequestMode mode = AssetRequestMode.AsyncLoad)
		{
			var asset = new Asset<T>(assetPath, source);

			assets[assetPath] = asset;

			asset.Request(mode);

			return asset;
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
			AddAssetReader(new TextReader());
		}

		private static void AutoloadAssets()
		{
			object[] parameterArray = new object[1];
			var autoloadAssetsMethod = typeof(Assets).GetMethod(nameof(AutoloadAssetsGeneric), BindingFlags.Static | BindingFlags.NonPublic);

			foreach (var pair in ReadersByType) {
				var type = pair.Key;
				var readers = pair.Value;

				foreach (var reader in readers) {
					if (!reader.AutoloadAssets) {
						continue;
					}

					parameterArray[0] = reader;
					autoloadAssetsMethod
						.MakeGenericMethod(type)
						.Invoke(null, parameterArray);
				}
			}
		}

		private static void AutoloadAssetsGeneric<T>(IAssetReader<T> reader)
		{
			foreach (var source in sources) {
				foreach (string assetPath in source.EnumerateAssets()) {
					string extension = Path.GetExtension(assetPath);

					if (reader.Extensions.Contains(extension)) {
						RequestFromSource<T>(source, assetPath, AssetRequestMode.ImmediateLoad); //TODO: Use async load, then wait for all of them to finish.
					}
				}
			}
		}
	}
}
