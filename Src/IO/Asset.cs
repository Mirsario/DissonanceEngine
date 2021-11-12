using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dissonance.Engine.IO
{
	/// <summary>
	/// Acts as a handle to an asset the load state of which may differ. Allows for delayed loading and potential unloading of assets.
	/// </summary>
	public abstract class Asset
	{
		/// <summary> The name of this asset. Used in registry. </summary>
		public string Name { get; }

		/// <summary> The virtual path this asset was created with. Can be null. </summary>
		public string AssetPath { get; internal init; }

		/// <summary> The state of this asset. </summary>
		public AssetState State { get; internal set; }

		/// <summary> The file source of this asset. Can be null. </summary>
		public AssetFileEntry? File { get; internal set; }

		/// <summary> Whether or not this asset is currently being loaded. </summary>
		public bool IsLoading => State == AssetState.Loading;

		/// <summary> Whether or not this asset has been loaded. </summary>
		public bool IsLoaded => State == AssetState.Loaded;

		internal Action Wait { get; set; }
		internal Action Continuation { get; set; }

		internal Asset(string name)
		{
			if (string.IsNullOrWhiteSpace(name) || name.Any(c => char.IsWhiteSpace(c))) {
				throw new ArgumentException("Asset name cannot be null, empty, or contain whitespaces.", nameof(name));
			}

			Name = name;
		}
	}

	/// <inheritdoc/>
	public sealed class Asset<T> : Asset
	{
		private T value;

		public T Value {
			get {
				if (!IsLoaded) {
					throw new InvalidOperationException("Asset has not yet been loaded.");
				}

				return value;
			}
			internal set {
				if (value == null) {
					throw new ArgumentNullException(nameof(value));
				}

				this.value = value;
			}
		}

		internal Asset(string name) : base(name) { }

		public void Request(AssetRequestMode mode = AssetRequestMode.AsyncLoad)
		{
			if (IsLoaded || mode == AssetRequestMode.DoNotLoad) {
				return;
			}

			State = AssetState.Loading;

			var loadTask = Load(mode);

			Wait = () => SafelyWaitForLoad(loadTask, tracked: true);
		}

		public bool TryGetOrRequestValue(out T result)
		{
			if (State == AssetState.NotLoaded) {
				Request();
			}

			if (IsLoaded) {
				result = Value;

				return true;
			}

			result = default;

			return false;
		}

		public T GetValueImmediately()
		{
			if (!IsLoaded) {
				Request(AssetRequestMode.ImmediateLoad);
				Wait();
			}

			return Value;
		}

		private async Task Load(AssetRequestMode mode)
		{
			var asyncContext = new ContinuationScheduler(this);

			string extension = Path.GetExtension(AssetPath);
			var readerByExtension = Assets.AssetTypeData<T>.ReaderByExtension;

			if (readerByExtension.Count == 0) {
				throw new InvalidOperationException($"No asset reader found with a return type of '{typeof(T).Name}'.");
			}

			if (!readerByExtension.TryGetValue(extension, out var assetReader) && !readerByExtension.TryGetValue("*", out assetReader)) {
				throw new InvalidOperationException($"No asset reader found for file extension '{extension}'.");
			}

			if (mode == AssetRequestMode.AsyncLoad) {
				await Task.Yield(); // This transfers the method's execution to a worker thread.
			}

			Value = await assetReader.ReadAsset(File, new MainThreadCreationContext(asyncContext));

			State = AssetState.Loaded;
		}

		private void SafelyWaitForLoad(Task loadTask, bool tracked)
		{
			if (State == AssetState.Loaded)
				return;

			if (!loadTask.IsCompleted && Assets.IsMainThread) {
				while (Continuation == null) {
					Thread.Yield();
				}

				if (tracked) {
					lock (Assets.RequestLock) {
						Continuation();
					}
				} else {
					Continuation();
				}

				if (!loadTask.IsCompleted)
					throw new Exception($"Load task not completed after running continuations on main thread?");
			}

			loadTask.GetAwaiter().GetResult(); // throw any exceptions (and wait for completion if this is not the worker thread)

			if (State != AssetState.Loaded)
				throw new Exception("This should not have happened.");
		}
	}
}
