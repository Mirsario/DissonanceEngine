using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GameEngine
{
	internal static class InternalUtils
	{
		public static StringComparer strComparerInvariantIgnoreCase = StringComparer.InvariantCultureIgnoreCase;

		public static int GenContentId<T>(T instance,List<T> byIdList) where T : class
		{
			for(int i = 0;i<byIdList.Count;i++) {
				if(byIdList[i]==null) {
					byIdList[i] = instance;
					return i;
				}
			}

			byIdList.Add(instance);
			return byIdList.Count-1;
		}
		public static bool ObjectOrCollectionCall<T>(object obj,Action<T> call,bool throwError = true) where T : class
		{
			if(obj is T objT) {
				call(objT);
				return true;
			}

			if(!(obj is ICollection objCollection)) {
				if(throwError) {
					throw new Exception("The ''obj'' argument's type isn't "+typeof(T)+" nor a collection.");
				}
				return false;
			}

			bool result = false;
			foreach(var val in objCollection) {
				result |= ObjectOrCollectionCall(val,call);
			}

			return result;
		}

		//TODO: Get rid of this
		public static bool IsMicrosoftAssembly(Assembly assembly) => assembly.Location.EndsWith("mscorlib.dll")
			|| assembly.FullName.Contains("PublicKeyToken = b77a5c561934e089")
			|| assembly.FullName.Contains("PublicKeyToken = b03f5f7f11d50a3a")
			|| assembly.FullName.Contains("PublicKeyToken = 31bf3856ad364e35");
	}
}
