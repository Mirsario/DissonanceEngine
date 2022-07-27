namespace Dissonance.Engine.IO;

/// <summary>
/// Specifies the mode in which an asset should be loaded.
/// </summary>
public enum AssetRequestMode
{
	/// <summary> The asset will not be loaded, for now. </summary>
	DoNotLoad,
	/// <summary> The asset will be loaded asynchronously on a separate thread. </summary>
	AsyncLoad,
	/// <summary> The asset will be loaded synchronously on the current thread, which may cause it to momentarily freeze. </summary>
	ImmediateLoad,
}
