using System.IO;

namespace Dissonance.Engine.IO
{
	public interface IAssetReader<out T>
	{
		/// <summary>
		/// The file extensions that this reader should be associated with.
		/// <br/> Avoid constant creation of new arrays when implementing this property:
		/// <code>public string[] Extensions { get; } = { ".extension" }; </code>
		/// </summary>
		string[] Extensions { get; }

		/// <summary>
		/// Whether or not this asset reader should be ran for every fitting asset early on.
		/// <br/> This is only evaluated when the reader is registered.
		/// </summary>
		bool AutoloadAssets => false;

		/// <summary>
		/// Reads the provided stream and returns a new instance of <typeparamref name="T"/> based on it and the provided <paramref name="assetPath"/>.
		/// </summary>
		/// <param name="stream"> The stream to read from. </param>
		/// <param name="assetPath"> The path of the asset that's currently being loaded. </param>
		/// <returns> A result of type <see cref="T"/>. </returns>
		T ReadFromStream(Stream stream, string assetPath);
	}
}
