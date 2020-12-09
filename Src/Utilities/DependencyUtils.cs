using System;
using System.Collections.Generic;

namespace Dissonance.Engine.Utilities
{
	public static class DependencyUtils
	{
		public static IEnumerable<T> DependencySort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies, bool throwOnRecursion = true)
		{
			var sorted = new List<T>();
			var visited = new HashSet<T>();

			foreach(var item in source) {
				DependencySortRecursion(item, visited, sorted, dependencies, throwOnRecursion);
			}

			return sorted;
		}

		private static void DependencySortRecursion<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencies, bool throwOnRecursion)
		{
			if(visited.Contains(item)) {
				if(throwOnRecursion && !sorted.Contains(item)) {
					throw new Exception($"Recursive dependency found in type '{item.GetType().Name}'");
				}

				return;
			}

			visited.Add(item);

			var dependenciesList = dependencies(item);

			if(dependenciesList != null) {
				foreach(var dep in dependenciesList) {
					if(dep != null) {
						DependencySortRecursion(dep, visited, sorted, dependencies, throwOnRecursion);
					}
				}
			}

			sorted.Add(item);
		}
	}
}
