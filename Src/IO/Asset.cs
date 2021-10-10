using System;

namespace Dissonance.Engine.IO
{
	public abstract class Asset
	{
		/// <summary> The path identifier of this asset. </summary>
		public string Name { get; internal set; }

		/// <summary> The state of this asset. </summary>
		public AssetState State { get; internal set; }

		/// <summary> The source of this asset. </summary>
		public AssetSource Source { get; internal set; }

		/// <summary> Whether or not this asset is currently being loaded. </summary>
		public bool IsLoading => State == AssetState.Loading;

		/// <summary> Whether or not this asset has been loaded. </summary>
		public bool IsLoaded => State == AssetState.Loaded;

		internal Asset(string name, AssetSource source)
		{
			Name = name;
			Source = source;
		}

		public static Asset<T> FromValue<T>(string name, T value)
		{
			return new Asset<T>(name, null) {
				Value = value,
				State = AssetState.Loaded,
			};
		}
	}

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

		internal Asset(string name, AssetSource source) : base(name, source) { }

		public void Request(AssetRequestMode mode = AssetRequestMode.AsyncLoad)
		{
			if (IsLoaded || mode == AssetRequestMode.None) {
				return;
			}

			if (!Enum.IsDefined(mode)) {
				throw new ArgumentOutOfRangeException(nameof(mode));
			}
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
			}

			return Value;
		}
	}
}
