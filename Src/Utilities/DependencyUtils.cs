using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable

namespace Dissonance.Engine.Utilities;

public static class DependencyUtils
{
	public delegate IEnumerable<int>? DependencyIndexGetter<T>(T arg);

	public static void DependencySort<T>(this List<T> source, DependencyIndexGetter<T> dependencyIndicesGetter, bool throwOnRecursion = true)
		=> DependencySort(CollectionsMarshal.AsSpan(source), dependencyIndicesGetter, throwOnRecursion);

	public static void DependencySort<T>(this T[] source, DependencyIndexGetter<T> dependencyIndicesGetter, bool throwOnRecursion = true)
		=> DependencySort((Span<T>)source, dependencyIndicesGetter, throwOnRecursion);

	public static unsafe void DependencySort<T>(this Span<T> source, DependencyIndexGetter<T> dependencyIndicesGetter, bool throwOnRecursion = true)
	{
		// Up to 8KB total
		const int MaxStackAllocationForBools = 1024 * 2;
		const int MaxStackAllocationForEntries = 1024 * 6;

		int length = source.Length;
		int tAllocationSize = Unsafe.SizeOf<T>() * length;
		Span<T> sorted;

		if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>() && tAllocationSize < MaxStackAllocationForEntries) {
			byte* byteAllocation = stackalloc byte[tAllocationSize];

			sorted = new Span<T>(byteAllocation, length);
		} else {
			sorted = new T[length];
		}

		Span<bool> boolAllocation = length * 2 < MaxStackAllocationForBools ? stackalloc bool[length * 2] : new bool[length * 2];
		Span<bool> visited = boolAllocation.Slice(0, length);
		Span<bool> isDefined = boolAllocation.Slice(length, length);

		int nextIndex = 0;

		for (int i = 0; i < length; i++) {
			DependencySortRecursion(source, i, ref nextIndex, sorted, visited, isDefined, dependencyIndicesGetter, throwOnRecursion);
		}

		sorted.CopyTo(source);
	}

	private static void DependencySortRecursion<T>(ReadOnlySpan<T> source, int index, ref int nextIndex, Span<T> sorted, Span<bool> visited, Span<bool> isDefined, DependencyIndexGetter<T> dependencyIndicesGetter, bool throwOnRecursion)
	{
		var item = source[index];

		if (visited[index]) {
			if (throwOnRecursion && !isDefined[index]) {
				throw new Exception($"Recursive dependency found in type '{item?.GetType().Name ?? "null"}'");
			}

			return;
		}

		visited[index] = true;

		var dependencyIndices = dependencyIndicesGetter(item);

		if (dependencyIndices != null) {
			foreach (int dependencyIndex in dependencyIndices) {
				DependencySortRecursion(source, dependencyIndex, ref nextIndex, sorted, visited, isDefined, dependencyIndicesGetter, throwOnRecursion);
			}
		}

		sorted[nextIndex++] = item;
		isDefined[index] = true;
	}
}
