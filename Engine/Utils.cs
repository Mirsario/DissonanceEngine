using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace GameEngine
{
	public static class Utils
	{
		public static StringComparer strComparerInvariantIgnoreCase = StringComparer.InvariantCultureIgnoreCase;
		
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
		
		public static bool IsMicrosoftAssembly(Assembly assembly)
		{
			return assembly.Location.EndsWith("mscorlib.dll")
			|| assembly.FullName.Contains("PublicKeyToken = b77a5c561934e089")
			|| assembly.FullName.Contains("PublicKeyToken = b03f5f7f11d50a3a")
			|| assembly.FullName.Contains("PublicKeyToken = 31bf3856ad364e35");
		}
	}
}
