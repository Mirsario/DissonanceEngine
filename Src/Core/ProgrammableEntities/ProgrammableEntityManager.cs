using System;
using System.Reflection;
using System.Collections.Generic;
using Dissonance.Engine.Core.Modules;
using System.Collections.Concurrent;

namespace Dissonance.Engine.Core.ProgrammableEntities
{
	internal sealed class ProgrammableEntityManager : EngineModule
	{
		internal static List<Action>[] hooks;
		internal static Dictionary<string,int> hookNameToId;
		internal static ConcurrentDictionary<Type,MethodInfo[]> hookMethodsCache;

		protected override void Init()
		{
			hookMethodsCache ??= new ConcurrentDictionary<Type,MethodInfo[]>();

			if(hookNameToId==null) {
				hookNameToId ??= new Dictionary<string,int>();

				Type type = typeof(ProgrammableEntity);
				var methods = type.GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);

				int id = 0;

				for(int i = 0;i<methods.Length;i++) {
					var method = methods[i];

					if(method.IsVirtual) {
						hookNameToId[method.Name] = id++;
					}
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
			var methods = GetHookMethodsByType(type);

			for(int i = 0;i<methods.Length;i++) {
				var methodInfo = methods[i];

				if(methodInfo!=null) {
					hooks[i].Add((Action)methodInfo.CreateDelegate(typeof(Action),entity));
				}
			}
		}
		public static void UnsubscribeEntity(ProgrammableEntity entity)
		{
			var type = entity.GetType();
			var methods = GetHookMethodsByType(type);

			for(int i = 0;i<methods.Length;i++) {
				var methodInfo = methods[i];

				if(methodInfo==null) {
					continue;
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

		private static MethodInfo[] GetHookMethodsByType(Type type)
		{
			if(!hookMethodsCache.TryGetValue(type,out var methods)) {
				methods = new MethodInfo[hooks.Length];

				foreach(var pair in hookNameToId) {
					string methodName = pair.Key;
					var method = type.GetMethod(methodName,BindingFlags.Public|BindingFlags.Instance);

					if(method==null) {
						throw new ArgumentException($"Method '{methodName}' does not exist in type '{type.Name}'.",nameof(methodName));
					}

					if(method.DeclaringType!=typeof(ProgrammableEntity)) {
						methods[pair.Value] = method;
					}
				}

				hookMethodsCache[type] = methods;
			}

			return methods;
		}
	}
}
