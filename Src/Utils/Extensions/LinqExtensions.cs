using System;
using System.Collections.Generic;

namespace Dissonance.Engine.Utils.Extensions
{
	public static class LinqExtensions
	{
		public static void CopyTo<TKey, TValue>(this Dictionary<TKey, TValue> from, Dictionary<TKey, TValue> to)
		{
			foreach(var pair in from) {
				to.Add(pair.Key, pair.Value);
			}
		}
		public static IEnumerable<TResult> SelectIgnoreNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			if(source == null) {
				throw new ArgumentNullException(nameof(source));
			}

			if(selector == null) {
				throw new ArgumentNullException(nameof(selector));
			}

			foreach(var element in source) {
				var result = selector(element);

				if(result != null) {
					yield return result;
				}
			}
		}
		public static bool TryGetFirst<T>(this IEnumerable<T> array, Predicate<T> predicate, out T result)
		{
			foreach(var val in array) {
				if(predicate(val)) {
					result = val;

					return true;
				}
			}

			result = default;

			return false;
		}
		public static bool TryFindIndex<T>(this T[] array, Predicate<T> predicate, out int index)
		{
			for(int i = 0; i < array.Length; i++) {
				if(predicate(array[i])) {
					index = i;

					return true;
				}
			}

			index = -1;

			return false;
		}
		public static bool TryFindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate, out int index)
		{
			for(int i = 0; i < list.Count; i++) {
				if(predicate(list[i])) {
					index = i;

					return true;
				}
			}

			index = -1;

			return false;
		}
	}
}
