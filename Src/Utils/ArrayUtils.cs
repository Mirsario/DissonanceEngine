using System;

namespace Dissonance.Engine.Utils
{
	public static class ArrayUtils
	{
		public static void Add<T>(ref T[] array,T value)
		{
			int originalLength = array?.Length ?? 0;

			Array.Resize(ref array,originalLength+1);

			array[originalLength] = value;
		}
		public static void Remove<T>(ref T[] array,int index)
		{
			int arrayLength = array?.Length ?? 0;

			if(arrayLength<=1 || index<0 || index>=arrayLength) {
				array = new T[0];

				return;
			}

			T[] newArray = new T[arrayLength-1];

			Array.Copy(array,newArray,arrayLength-1);

			if(index<arrayLength) {
				Array.Copy(array,index+1,newArray,index,arrayLength-(index+1));
			}

			array = newArray;
		}
	}
}
