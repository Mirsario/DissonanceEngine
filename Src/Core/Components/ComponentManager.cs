using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public class ComponentManager : EngineModule
	{
		internal static ComponentManager Instance => Game.Instance.GetModule<ComponentManager>(true);

		private ComponentTypeInfo[] sortedComponentTypeInfo = Array.Empty<ComponentTypeInfo>();
		private Dictionary<Type, ComponentTypeInfo> componentInfoByType = new Dictionary<Type, ComponentTypeInfo>();
		private Type[] componentTypes;
		private List<HookList> hookLists;

		internal HookList HookFixedUpdate { get; private set; } = new HookList(typeof(Component).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic));
		internal HookList HookRenderUpdate { get; private set; } = new HookList(typeof(Component).GetMethod("RenderUpdate", BindingFlags.Instance | BindingFlags.NonPublic));
		internal HookList HookOnGUI { get; private set; } = new HookList(typeof(Component).GetMethod("OnGUI", BindingFlags.Instance | BindingFlags.NonPublic));

		protected override void PreInit()
		{
			hookLists = new List<HookList> {
				HookFixedUpdate,
				HookRenderUpdate,
				HookOnGUI
			};

			AssemblyRegistrationModule.OnAssemblyRegistered += (assembly, types) => {
				foreach(var type in types) {
					if(!typeof(Component).IsAssignableFrom(type)) {
						continue;
					}

					componentInfoByType.Add(type, new ComponentTypeInfo(type));
				}

				UpdateComponentTypeData();
			};
		}
		protected override void FixedUpdate()
		{
			var instance = Instance;
			int[] hookIndices = instance.HookFixedUpdate.ValidTypeIndices;

			for(int i = 0; i < hookIndices.Length; i++) {
				var instances = instance.sortedComponentTypeInfo[i].exactInstances.enabled;

				for(int j = 0; j < instances.Count; j++) {
					instances[j].FixedUpdate();
				}
			}
		}
		protected override void RenderUpdate()
		{
			var instance = Instance;
			int[] hookIndices = instance.HookRenderUpdate.ValidTypeIndices;

			for(int i = 0; i < hookIndices.Length; i++) {
				var instances = instance.sortedComponentTypeInfo[i].exactInstances.enabled;

				for(int j = 0; j < instances.Count; j++) {
					instances[j].RenderUpdate();
				}
			}
		}
		protected override void OnDispose()
		{
			if(componentInfoByType != null) {
				componentInfoByType.Clear();

				componentInfoByType = null;
			}

			sortedComponentTypeInfo = null;
		}

		//Count
		public static int CountComponents<T>(bool? enabled = true, bool exactType = false)
			=> CountComponents(typeof(T), exactType, enabled);
		public static int CountComponents(Type type, bool exactType = false, bool? enabled = true)
			=> GetComponentsList(type, exactType, enabled)?.Count ?? 0;
		//Enumerate
		public static IEnumerable<Component> EnumerateComponents(Type type, bool exactType = false, bool? enabled = true)
			=> GetComponentsList(type, exactType, enabled) ?? Enumerable.Empty<Component>();
		public static IEnumerable<T> EnumerateComponents<T>(bool exactType = false) where T : Component
		{
			foreach(var component in EnumerateComponents(typeof(T), exactType)) {
				yield return (T)component;
			}
		}

		//TODO: OnGUI hook should be removed forever in favor of adequate non-immediate rendering.
		internal static void OnGUI()
		{
			var instance = Instance;
			int[] hookIndices = instance.HookOnGUI.ValidTypeIndices;

			for(int i = 0; i < hookIndices.Length; i++) {
				var instances = instance.sortedComponentTypeInfo[i].exactInstances.enabled;

				for(int j = 0; j < instances.Count; j++) {
					instances[j].OnGUI();
				}
			}
		}
		internal static void ModifyInstanceLists(Type type, Action<InstanceLists<Component>> action)
		{
			var instance = Instance;

			action(instance.componentInfoByType[type].exactInstances);

			//TODO: This enumeration could've been made faster.
			foreach(var baseType in ReflectionUtils.EnumerateBaseTypes(type, true, typeof(Component))) {
				action(instance.componentInfoByType[baseType].sharedInstances);
			}
		}

		private static IReadOnlyList<Component> GetComponentsList(Type type, bool exactType = false, bool? enabled = true)
		{
			if(!Instance.componentInfoByType.TryGetValue(type, out var info)) {
				return null;
			}

			var lists = exactType ? info.exactInstances : info.sharedInstances;

			return enabled switch
			{
				true => lists.enabledReadOnly,
				false => lists.disabledReadOnly,
				_ => lists.allReadOnly
			};
		}
		private static void UpdateComponentTypeData()
		{
			var instance = Instance;

			//TODO:
			instance.sortedComponentTypeInfo = instance.componentInfoByType.Values.ToArray();
			instance.componentTypes = instance.sortedComponentTypeInfo.Select(i => i.type).ToArray();

			for(int i = 0; i < instance.hookLists.Count; i++) {
				instance.hookLists[i].Update(instance.componentTypes);
			}
		}
	}
}
