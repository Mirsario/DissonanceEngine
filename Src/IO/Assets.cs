using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Dissonance.Engine.IO
{
	//TODO: Make a ton more threadsafe.
	public sealed class Assets : EngineModule
	{
		internal static class AssetTypeData<T>
		{
			public static readonly Dictionary<string, Asset<T>> Assets = new();
			public static readonly HashSet<IAssetManager<T>> Managers = new();
			public static readonly HashSet<IAssetReader<T>> Readers = new();
			public static readonly Dictionary<string, IAssetReader<T>> ReaderByExtension = new();
		}

		internal const string BuiltInAssetsDirectory = "BuiltInAssets";

		internal static readonly object RequestLock = new();
		internal static readonly HashSet<Type> ReaderAssetTypes = new();
		internal static readonly ConcurrentQueue<Action> AssetTransferQueue = new();

		private static readonly HashSet<AssetSource> sources = new();
		private static readonly Dictionary<string, AssetFileEntry> assetFiles = new();
		//private static readonly Dictionary<Type, IDictionary<string, Asset>> assets = new();

		internal static bool IsMainThread => Thread.CurrentThread == Game.MainThread;

		protected override void Init()
		{
			RegisterAssetSources();
			RegisterAssetReaders();
			PrepareAssets();
		}

		protected override void PreRenderUpdate()
		{
			while (!AssetTransferQueue.IsEmpty) {
				if (AssetTransferQueue.TryDequeue(out Action continuation)) {
					continuation();
				}
			}
		}

		/// <summary>
		/// Returns whether or not an asset of the provided type and case-sensitive virtual path exists.
		/// </summary>
		/// <param name="assetPath"> The path of the asset. This path is virtual and case-sensitive - system paths will not work. </param>
		public static bool Exists<T>(string assetPath)
		{
			return assetFiles.ContainsKey(assetPath);
		}

		/// <summary>
		/// Attempts to get or create an asset with the provided case-sensitive virtual asset path and base path for the former path to optionally be relative to.
		/// <br/> Optionally, may also request the asset to be loaded with the provided mode.
		/// </summary>
		/// <param name="basePath"> The base path to attempt putting in front of <paramref name="assetPath"/>. </param>
		/// <inheritdoc cref="Get{T}(string, AssetRequestMode)"/>
		public static Asset<T> Get<T>(string assetPath, string basePath, AssetRequestMode mode = AssetRequestMode.DoNotLoad)
		{
			if (TryGet<T>(assetPath, basePath, out var result, mode)) {
				return result;
			}

			throw new KeyNotFoundException($"Couldn't find asset '{assetPath}' in any available content sources. Base path: '{basePath}'.");
		}

		/// <summary>
		/// Attempts to get or create an asset with the provided case-sensitive virtual asset path, optionally requesting it to be loaded with the provided mode.
		/// <br/> Throws exceptions on failure.
		/// </summary>
		/// <returns> <see cref="Asset"/>&lt;<typeparamref name="T"/>&gt; - an asset handle. </returns>
		/// <exception cref="ArgumentOutOfRangeException"> Asset couldn't be found. </exception>
		/// <inheritdoc cref="TryGet{T}(string, out Asset{T}, AssetRequestMode)"/>
		public static Asset<T> Get<T>(string assetPath, AssetRequestMode mode = AssetRequestMode.DoNotLoad)
		{
			if (TryGet<T>(assetPath, out var result, mode)) {
				return result;
			}

			throw new KeyNotFoundException($"Couldn't find asset '{assetPath}' in any available content sources.");
		}

		/// <summary>
		/// Safely attempts to get or create an asset with the provided case-sensitive virtual asset path and base path for the former path to optionally be relative to.
		/// <br/> Optionally, may also request the asset to be loaded with the provided mode.
		/// </summary>
		/// <param name="basePath"> The base path to attempt putting in front of <paramref name="assetPath"/>. </param>
		/// <inheritdoc cref="TryGet{T}(string, out Asset{T}, AssetRequestMode)"/>
		public static bool TryGet<T>(string assetPath, string basePath, out Asset<T> result, AssetRequestMode mode = AssetRequestMode.DoNotLoad)
		{
			bool rootPath = assetPath.StartsWith('/');

			if (!rootPath && TryGet<T>(FilterPath(Path.Combine(basePath, assetPath)), out result, mode)) {
				return true;
			}

			if (rootPath) {
				assetPath = assetPath.Substring(1);
			}

			return TryGet(assetPath, out result, mode);
		}

		/// <summary>
		/// Safely attempts to get or create an asset with the provided case-sensitive virtual asset path.
		/// <br/> Optionally, may also request the asset to be loaded with the provided mode.
		/// </summary>
		/// <typeparam name="T"> The type of the asset. </typeparam>
		/// <param name="assetPath"> The path of the asset. This path is virtual and case-sensitive - system paths will not work. </param>
		/// <param name="result"> The resulting <see cref="Asset"/>&lt;<typeparamref name="T"/>&gt; - an asset handle, if it was found. </param>
		/// <param name="mode"> The mode to request the asset's loading with. Defaults to <see cref="AssetRequestMode.DoNotLoad"/>. </param>
		/// <returns> A boolean indicating whether the operation succeeded. </returns>
		public static bool TryGet<T>(string assetPath, out Asset<T> result, AssetRequestMode mode = AssetRequestMode.DoNotLoad)
		{
			if (AssetTypeData<T>.Assets.TryGetValue(assetPath, out var cachedAsset) && cachedAsset is Asset<T> cachedAssetResult) {
				if (mode != AssetRequestMode.DoNotLoad && cachedAssetResult.State == AssetState.NotLoaded) {
					cachedAssetResult.Request(mode);
				}

				result = cachedAssetResult;

				if (mode == AssetRequestMode.ImmediateLoad && result.State != AssetState.Loaded) {
					result.Wait();
				}

				return true;
			}

			if (assetFiles.TryGetValue(assetPath, out var assetFile)) {
				result = CreateAsset<T>(assetFile);

				if (mode != AssetRequestMode.DoNotLoad) {
					result.Request(mode);
				}

				if (mode == AssetRequestMode.ImmediateLoad && result.State != AssetState.Loaded) {
					result.Wait();
				}

				return true;
			}

			result = default;

			return false;
		}

		/// <summary>
		/// Attempts to find a registered asset using its case-sensitive name instead of a path.
		/// <br/> Throws exceptions on failure.
		/// </summary>
		/// <typeparam name="T"> The type of the asset. </typeparam>
		/// <param name="assetName"> The case-sensitive name of the asset. This is not the same as its path. </param>
		/// <returns> <see cref="Asset"/>&lt;<typeparamref name="T"/>&gt; - an asset handle. </returns>
		/// <exception cref="KeyNotFoundException"> No registered asset could be found with the provided name. </exception>
		public static Asset<T> Find<T>(string assetName, AssetRequestMode mode = AssetRequestMode.DoNotLoad)
			=> AssetLookup<T>.Get(assetName, mode);

		/// <summary> Safely attempts to find a registered asset using its case-sensitive name instead of a path. </summary>
		/// <typeparam name="T"> The type of the asset. </typeparam>
		/// <param name="assetName"> The case-sensitive name of the asset. This is not the same as its path. </param>
		/// <param name="result"> The resulting <see cref="Asset"/>&lt;<typeparamref name="T"/>&gt; - an asset handle, if it was found. </param>
		/// <returns> A boolean indicating whether the operation succeeded. </returns>
		public static bool TryFind<T>(string assetName, out Asset<T> result, AssetRequestMode mode = AssetRequestMode.DoNotLoad)
			=> AssetLookup<T>.TryGet(assetName, out result, mode);

		/// <summary>
		/// Creates, registers and returns a new pre-loaded asset object with the provided name and value.
		/// <br/> Pre-loaded assets cannot be reloaded and have a value by default.
		/// </summary>
		/// <typeparam name="T"> The type of the asset. </typeparam>
		/// <param name="name"> The name that the created asset should be made and registered with. </param>
		/// <param name="path"> The virtual path using which this asset has been created. </param>
		/// <param name="value"> The value that the created asset should have. Must not be null. </param>
		public static Asset<T> CreateLoaded<T>(string name, T value)
		{
			var asset = CreateUntracked(name, value);

			AssetLookup<T>.Register(name, null, asset);

			return asset;
		}

		/// <summary>
		/// Creates and returns a new untracked pre-loaded asset object with the provided name and value.
		/// <br/> Pre-loaded assets cannot be reloaded and have a value by default.
		/// </summary>
		/// <typeparam name="T"> The type of the asset. </typeparam>
		/// <param name="name"> The name that the created asset should have. </param>
		/// <param name="path"> The virtual path using which this asset has been created. </param>
		/// <param name="value"> The value that the created asset should have. Must not be null. </param>
		public static Asset<T> CreateUntracked<T>(string name, T value)
		{
			if (value is null) {
				throw new ArgumentNullException(nameof(value));
			}

			return new Asset<T>(name) {
				Value = value,
				State = AssetState.Loaded,
			};
		}

		/// <summary> Returns the provided path with directory separators fixed. </summary>
		public static string FilterPath(string path)
		{
			path = path.Replace('\\', '/');

			return path;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="assetSource"> </param>
		public static void AddAssetSource(AssetSource assetSource)
		{
			sources.Add(assetSource);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="assetReader"> </param>
		/// <exception cref="InvalidOperationException"> </exception>
		public static void AddAssetReader<T>(IAssetReader<T> assetReader)
		{
			if (!AssetTypeData<T>.Readers.Add(assetReader)) {
				throw new InvalidOperationException($"Asset reader '{assetReader.GetType().Name}' is already registered.");
			}

			ReaderAssetTypes.Add(typeof(T));

			foreach (string extension in assetReader.Extensions) {
				AssetTypeData<T>.ReaderByExtension.Add(extension, assetReader);
			}
		}

		private static Asset<T> CreateAsset<T>(AssetFileEntry assetFile)
		{
			string assetPath = assetFile.Path;
			string assetName = Path.GetFileNameWithoutExtension(assetPath);

			var asset = new Asset<T>(assetName) {
				AssetPath = assetPath,
				File = assetFile
			};

			AssetLookup<T>.Register(assetName, assetFile.Path, asset);

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
			//TODO: Create a common system/attribute for handling autoloading/autoregistration of types and use it here to allow game assemblies to benefit too.
			var addAssetReaderMethod = typeof(Assets).GetMethod(nameof(AddAssetReader), BindingFlags.Static | BindingFlags.Public);
			object[] argumentArray = new object[1];

			foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
				if (type.IsAbstract) {
					continue;
				}

				object instance = null;

				foreach (var interfaceType in type.GetInterfaces()) {
					if (!interfaceType.IsGenericType || interfaceType.GetGenericTypeDefinition() != typeof(IAssetReader<>)) {
						continue;
					}

					var interfaceTypeArgument = interfaceType.GetGenericArguments()[0];

					if (instance == null) {
						instance = Activator.CreateInstance(type, true);
						argumentArray[0] = instance;
					}

					addAssetReaderMethod
						.MakeGenericMethod(interfaceTypeArgument)
						.Invoke(null, argumentArray);
				}
			}
		}

		private static void PrepareAssets()
		{
			PrepareAssetFileLists();
			PrepareAssetLookup();
			AutoloadAssets();
		}

		private static void PrepareAssetFileLists()
		{
			assetFiles.Clear();

			foreach (var source in sources) {
				foreach (string assetPath in source.EnumerateAssets()) {
					assetFiles[assetPath] = new AssetFileEntry(assetPath, source);
				}
			}
		}

		private static void PrepareAssetLookup()
		{
			var refreshAssetLookupMethod = typeof(Assets).GetMethod(nameof(RefreshAssetLookupOfType), BindingFlags.Static | BindingFlags.NonPublic);

			foreach (var type in ReaderAssetTypes) {
				refreshAssetLookupMethod
					.MakeGenericMethod(type)
					.Invoke(null, null);
			}
		}

		private static void RefreshAssetLookupOfType<T>()
		{
			foreach (var assetFile in assetFiles.Values) {
				string assetPath = assetFile.Path;
				string assetExtension = Path.GetExtension(assetPath);

				if (!AssetTypeData<T>.ReaderByExtension.TryGetValue(assetExtension, out var assetReader)) {
					continue;
				}

				string assetName = Path.GetFileNameWithoutExtension(assetPath);

				AssetLookup<T>.Register(assetName, assetPath, null);
			}
		}

		private static void AutoloadAssets()
		{
			var autoloadAssetsMethod = typeof(Assets).GetMethod(nameof(AutoloadAssetsGeneric), BindingFlags.Static | BindingFlags.NonPublic);

			foreach (var type in ReaderAssetTypes) {
				autoloadAssetsMethod
					.MakeGenericMethod(type)
					.Invoke(null, null);
			}
		}

		private static void AutoloadAssetsGeneric<T>()
		{
			var loadingAssets = new Queue<Asset>();

			foreach (var reader in AssetTypeData<T>.Readers) {
				if (!reader.AutoloadAssets) {
					continue;
				}

				foreach (var assetFile in assetFiles.Values) {
					string extension = Path.GetExtension(assetFile.Path);

					if (reader.Extensions.Contains(extension)) {
						var asset = CreateAsset<T>(assetFile);

						asset.Request(AssetRequestMode.AsyncLoad);
						loadingAssets.Enqueue(asset);
					}
				}

				while (loadingAssets.TryDequeue(out var asset)) {
					if (asset.State == AssetState.Loading) {
						asset.Wait();
					}
				}
			}
		}
	}
}
