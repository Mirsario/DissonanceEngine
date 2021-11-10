using System.IO;
using System.Threading.Tasks;

namespace Dissonance.Engine.IO
{
	public interface IAssetReader<T>
	{
		/// <summary>
		/// The file extensions that this reader should be associated with.
		/// <br/> Avoid constant creation of new arrays when implementing this property:
		/// <code>public string[] Extensions { get; } = { ".extension" }; </code>
		/// </summary>
		string[] Extensions { get; }

		/// <summary>
		/// Reads the provided stream and returns a new instance of <typeparamref name="T"/> based on it and the provided <paramref name="assetPath"/>.
		/// </summary>
		/// <param name="stream"> The stream to read from. </param>
		/// <param name="assetPath"> The path of the asset that's currently being loaded. </param>
		/// <param name="switchToMainThread"> Await this to switch execution of the method to the main thread. </param>
		/// <returns> A result of type <see cref="T"/>. </returns>
		ValueTask<T> ReadFromStream(Stream stream, string assetPath, MainThreadCreationContext switchToMainThread);
	}
}
