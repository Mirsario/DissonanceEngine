using System;

namespace Dissonance.Engine.Utilities
{
	public static class ArrayUtils
	{
		public static void Add<T>(ref T[] array, T value)
		{
			int originalLength = array?.Length ?? 0;

			Array.Resize(ref array, originalLength + 1);

			array[originalLength] = value;
		}
		public static void Remove<T>(ref T[] array, int index)
		{
			int arrayLength = array?.Length ?? 0;

			if(arrayLength <= 1 || index < 0 || index >= arrayLength) {
				array = new T[0];

				return;
			}

			T[] newArray = new T[arrayLength - 1];

			Array.Copy(array, newArray, arrayLength - 1);

			if(index < arrayLength) {
				Array.Copy(array, index + 1, newArray, index, arrayLength - (index + 1));
			}

			array = newArray;
		}

		/// <summary>
		/// Resizes an array while setting new indices' values to the provided default value.
		/// </summary>
		public static void ResizeAndFillArray<T>(ref T[] array, int newLength, T defaultValue)
		{
			if(newLength < 0) {
				throw new ArgumentException($"{nameof(newLength)} must be more than or equal to 0.");
			}

			if(array == null) {
				array = new T[newLength];

				for(int i = 0; i < newLength; i++) {
					array[i] = defaultValue;
				}
			}

			int oldLength = array.Length;

			if(newLength == oldLength) {
				return;
			}

			var newArray = new T[newLength];

			if(newLength < array.Length) {
				for(int i = 0; i < newLength; i++) {
					newArray[i] = array[i];
				}
			} else {
				for(int i = 0; i < oldLength; i++) {
					newArray[i] = array[i];
				}

				for(int i = oldLength; i < newLength; i++) {
					newArray[i] = defaultValue;
				}
			}

			array = newArray;
		}
	}
}
