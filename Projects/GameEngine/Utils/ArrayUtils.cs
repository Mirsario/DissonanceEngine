using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Utils
{
	public static class ArrayUtils
	{
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
	}
}
