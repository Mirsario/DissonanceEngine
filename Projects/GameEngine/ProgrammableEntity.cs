using System;
using System.Reflection;
using System.Collections.Generic;

namespace GameEngine
{
	public abstract class ProgrammableEntity //TODO: This is one really stupid name
	{
		public virtual void FixedUpdate() {}
		public virtual void RenderUpdate() {}
		public virtual void OnGUI() {}
	}
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
					hookNameToId[methods[i].Name] = id++;
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
				if(methodInfo==null) {
					continue;
				}
				hooks[i].Add((Action)methodInfo.CreateDelegate(typeof(Action),entity));
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
					var action = arr[j];
					if(action.Target==entity) {
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

	internal class ProgrammableEntityTypeInfo //TODO: This especially
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
