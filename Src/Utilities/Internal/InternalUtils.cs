using System;
using System.Collections;
using System.Collections.Generic;

namespace Dissonance.Engine.Utilities
{
	internal static class InternalUtils
	{
		public static readonly StringComparer DefaultStringComparer = StringComparer.InvariantCultureIgnoreCase;

		public static int GenContentId<T>(T instance, List<T> byIdList) where T : class
		{
			for (int i = 0; i < byIdList.Count; i++) {
				if (byIdList[i] == null) {
					byIdList[i] = instance;

					return i;
				}
			}

			byIdList.Add(instance);

			return byIdList.Count - 1;
		}

		public static bool ObjectOrCollectionCall<T>(object obj, Action<T> call, bool throwError = true) where T : class
		{
			if (obj is T objT) {
				call(objT);
				return true;
			}

			if (!(obj is ICollection objCollection)) {
				if (throwError) {
					throw new Exception($"The '{nameof(obj)}' argument's type isn't '{typeof(T).Name}' nor a collection.");
				}

				return false;
			}

			bool result = false;

			foreach (object val in objCollection) {
				result |= ObjectOrCollectionCall(val, call);
			}

			return result;
		}
	}
}
