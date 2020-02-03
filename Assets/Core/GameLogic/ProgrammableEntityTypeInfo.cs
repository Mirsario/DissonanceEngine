using System;
using System.Reflection;

namespace GameEngine
{
	internal class ProgrammableEntityTypeInfo //TODO: Come up with a better name?
	{
		public MethodInfo[] methodInfos;

		public ProgrammableEntityTypeInfo(Type type)
		{
			MethodInfo GetMethodInfo(string methodName)
			{
				var method = type.GetMethod(methodName,BindingFlags.Public|BindingFlags.Instance);
				if(method==null) {
					throw new ArgumentException($"Method '{methodName}' does not exist in type '{type.Name}'.",nameof(methodName));
				}
				if(method.DeclaringType==typeof(ProgrammableEntity)) {
					return null;
				}
				return method;
			}

			methodInfos = new MethodInfo[ProgrammableEntityHooks.hooks.Length];

			foreach(var pair in ProgrammableEntityHooks.hookNameToId) {
				methodInfos[pair.Value] = GetMethodInfo(pair.Key);
			}
		}
	}
}
