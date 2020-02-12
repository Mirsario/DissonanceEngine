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
			if(source==null) { throw new ArgumentNullException(nameof(source)); }
			if(selector==null) { throw new ArgumentNullException(nameof(selector)); }
			return SelectIgnoreNullIterator(source,selector);
		}
		private static IEnumerable<TResult> SelectIgnoreNullIterator<TSource, TResult>(IEnumerable<TSource> source,Func<TSource,TResult> selector)
		{
			foreach(var element in source) {
				var result = selector(element);
				if(result!=null) {
					yield return result;
				}
			}
		}
		public static bool TryFirst<T>(this IEnumerable<T> array,Func<T,bool> func,out T result)
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

		public static string AllToString<T>(this T[] array,string separator = ",")
		{
			if(array==null) {
				return "NULL";
			}
			string str = "";
			for(int i = 0;i<array.Length;i++) {
				str += array[i].ToString();
				if(i<array.Length-1) {
					str += separator;
				}
			}
			return str;
		}
	}
}
