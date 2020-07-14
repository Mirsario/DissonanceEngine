using System;
using System.Collections.Generic;

namespace Dissonance.Engine.Utils.Extensions
{
	public static class LinqExtensions
	{
		public static void CopyTo<TKey, TValue>(this Dictionary<TKey,TValue> from,Dictionary<TKey,TValue> to)
		{
			foreach(var pair in from) {
				to.Add(pair.Key,pair.Value);
			}
		}
		public static IEnumerable<TResult> SelectIgnoreNull<TSource, TResult>(this IEnumerable<TSource> source,Func<TSource,TResult> selector)
		{
			if(source==null) {
				throw new ArgumentNullException(nameof(source));
			}

			if(selector==null) {
				throw new ArgumentNullException(nameof(selector));
			}

			foreach(var element in source) {
				var result = selector(element);

				if(result!=null) {
					yield return result;
				}
			}
		}
		public static bool TryGetFirst<T>(this IEnumerable<T> array,Func<T,bool> func,out T result)
		{
			foreach(var val in array) {
				if(func(val)) {
					result = val;

					return true;
				}
			}

			result = default;

			return false;
		}
	}
}
