using System.Collections.Generic;

namespace Dissonance.Engine.IO
{
	public static class GameContent
	{
		public static void Register<T>(string name, T content)
			=> ContentLookup<T>.Register(name, content);

		/// <summary>
		/// Attempts to find a registered content piece using its case-sensitive name.
		/// <br/> Throws exceptions on failure.
		/// </summary>
		/// <typeparam name="T"> The type of the content. </typeparam>
		/// <param name="contentName"> The case-sensitive identifier of the content piece. This is not the same as its path. </param>
		/// <returns> <typeparamref name="T"/> - the content prefab. </returns>
		/// <exception cref="KeyNotFoundException"/>
		public static T Find<T>(string contentName)
			=> ContentLookup<T>.Get(contentName);

		/// <summary> Safely attempts to find a registered content piece using its case-sensitive identifier. </summary>
		/// <typeparam name="T"> The type of the asset. </typeparam>
		/// <param name="contentName"> The case-sensitive identifier of the content piece. This is not the same as its path. </param>
		/// <param name="result"> The resulting <typeparamref name="T"/>, if it was found. </param>
		/// <returns> A boolean indicating whether the operation succeeded. </returns>
		public static bool TryFind<T>(string contentName, out T result)
			=> ContentLookup<T>.TryGetValue(contentName, out result);
	}
}
