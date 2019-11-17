using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Utils
{
	public static class ArrayUtils
	{
		public delegate void RefAction<T>(ref T value);

		public static void Add<T>(ref T[] array,T value)
		{
			int arrayLength = array?.Length ?? 0;

			if(arrayLength==0) {
				array = new T[] { value };
				return;
			}

			T[] newArray = new T[arrayLength+1];
			Array.Copy(array,newArray,arrayLength);
			newArray[arrayLength] = value;

			array = newArray;
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
		public static void ResizeAdvanced<T>(ref T[] array,int newSize,RefAction<T> onAdd,RefAction<T> onRemove)
		{
			if(newSize<0) {
				throw new ArgumentOutOfRangeException(nameof(newSize),$"{nameof(newSize)} is less than zero.");
			}

			int oldSize = array?.Length ?? 0;

			if(newSize<oldSize && onRemove!=null) {
				for(int i = newSize;i<oldSize;i++) {
					onRemove(ref array[i]);
				}
			}

			Array.Resize(ref array,newSize);

			if(newSize>oldSize && onAdd!=null) {
				for(int i = oldSize;i<newSize;i++) {
					onAdd(ref array[i]);
				}
			}
		}
	}
}
