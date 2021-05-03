using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine
{
	internal class HookList
	{
		public readonly MethodInfo Method;

		public int[] ValidTypeIndices { get; private set; } = Array.Empty<int>();

		public HookList(MethodInfo method)
		{
			Method = method;
		}

		public void Update(Type[] types)
		{
			var indexList = new List<int>();
			var baseDeclaringType = Method.DeclaringType;
			bool isInterface = baseDeclaringType.IsInterface;

			if(isInterface) {
				for(int i = 0; i < types.Length; i++) {
					if(baseDeclaringType.IsAssignableFrom(types[i])) {
						indexList.Add(i);
					}
				}
			} else {
				var argTypes = Method.GetParameters().Select(p => p.ParameterType).ToArray();

				for(int i = 0; i < types.Length; i++) {
					var currentMethod = types[i].GetMethod(Method.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, argTypes, null);

					if(currentMethod != null && currentMethod.DeclaringType != baseDeclaringType) {
						indexList.Add(i);
					}
				}
			}

			ValidTypeIndices = indexList.ToArray();
		}
	}
}
