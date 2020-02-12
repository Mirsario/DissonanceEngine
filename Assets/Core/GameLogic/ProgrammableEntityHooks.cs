using System;
using System.Reflection;
using System.Collections.Generic;

namespace Dissonance.Engine
{
	internal static class ProgrammableEntityHooks
	{
		internal static Dictionary<Type,ProgrammableEntityTypeInfo> typeToInfo;
		internal static Dictionary<string,int> hookNameToId;
		internal static List<Action>[] hooks;

		public static void Initialize()
		{
			typeToInfo = new Dictionary<Type,ProgrammableEntityTypeInfo>();
			hookNameToId = new Dictionary<string,int>();
			
			Type type = typeof(ProgrammableEntity);
			var methods = type.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);

			int id = 0;

			for(int i = 0;i<methods.Length;i++) {
				var method = methods[i];
				if(method.IsVirtual) {
					hookNameToId[method.Name] = id++;
				}
			}

			hooks = new List<Action>[hookNameToId.Count];

			for(int i = 0;i<hooks.Length;i++) {
				hooks[i] = new List<Action>();
			}
		}
		public static void SubscribeEntity(ProgrammableEntity entity)
		{
			var type = entity.GetType();

			if(!typeToInfo.TryGetValue(type,out var info)) {
				typeToInfo[type] = info = new ProgrammableEntityTypeInfo(type);
			}

			for(int i = 0;i<info.methodInfos.Length;i++) {
				var methodInfo = info.methodInfos[i];
				if(methodInfo!=null) {
					hooks[i].Add((Action)methodInfo.CreateDelegate(typeof(Action),entity));
				}
			}
		}
		public static void UnsubscribeEntity(ProgrammableEntity entity)
		{
			var type = entity.GetType();
			if(!typeToInfo.TryGetValue(type,out var info)) {
				typeToInfo[type] = info = new ProgrammableEntityTypeInfo(type);
			}

			for(int i = 0;i<info.methodInfos.Length;i++) {
				var methodInfo = info.methodInfos[i];

				if(methodInfo==null) {
					return;
				}

				var arr = hooks[i];
				for(int j = 0;j<arr.Count;j++) {
					if(arr[j].Target==entity) {
						arr.RemoveAt(j--);
					}
				}
			}
		}
		public static void InvokeHook(string name) => InvokeHook(hookNameToId[name]);
		public static void InvokeHook(int id)
		{
			var list = hooks[id];

			for(int i = 0;i<list.Count;i++) {
				list[i]();
			}
		}
	}
}
