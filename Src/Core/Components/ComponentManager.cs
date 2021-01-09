using System;
using System.Collections.Generic;
using System.Linq;
using Dissonance.Engine.Utilities;
using ComponentInstanceLists = Dissonance.Engine.InstanceLists<Dissonance.Engine.Component>;

namespace Dissonance.Engine
{
	public class ComponentManager : EngineModule
	{
		internal static ComponentManager Instance => Game.Instance.GetModule<ComponentManager>(true);

		private Dictionary<Type, ComponentInstanceLists> typeInstances = new Dictionary<Type, ComponentInstanceLists>();
		private Dictionary<Type, ComponentInstanceLists> exactTypeInstances = new Dictionary<Type, ComponentInstanceLists>();

		protected override void Init()
		{
			typeInstances = new Dictionary<Type, ComponentInstanceLists>();
			exactTypeInstances = new Dictionary<Type, ComponentInstanceLists>();
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

		internal static void ModifyInstanceLists(Type type, Action<ComponentInstanceLists> action)
		{
			static ComponentInstanceLists GetLists(Type key, Dictionary<Type, ComponentInstanceLists> dictionary)
			{
				if(!dictionary.TryGetValue(key, out var lists)) {
					dictionary[key] = lists = new ComponentInstanceLists();
				}

				return lists;
			}

			action(GetLists(type, Instance.exactTypeInstances));

			//TODO: This enumeration could've been made faster.
			foreach(var baseType in ReflectionUtils.EnumerateBaseTypes(type, true, typeof(Component))) {
				action(GetLists(baseType, Instance.typeInstances));
			}
		}

		private static IReadOnlyList<Component> GetComponentsList(Type type, bool exactType = false, bool? enabled = true)
		{
			if(!(exactType ? Instance.exactTypeInstances : Instance.typeInstances).TryGetValue(type, out var lists)) {
				return null;
			}

			return enabled switch
			{
				true => lists.enabledReadOnly,
				false => lists.disabledReadOnly,
				_ => lists.allReadOnly
			};
		}
	}
}
