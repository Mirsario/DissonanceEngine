using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.Core.Components.Attributes;
using Dissonance.Engine.Core.Modules;
using Dissonance.Engine.Utils.Internal;

namespace Dissonance.Engine.Core.Components
{
	public class ComponentManager : EngineModule
	{
		internal static ComponentManager Instance => Game.Instance.GetModule<ComponentManager>(true);

		private Dictionary<Type,List<Component>> typeInstances = new Dictionary<Type,List<Component>>();
		private Dictionary<Type,List<Component>> exactTypeInstances = new Dictionary<Type,List<Component>>();
		private Dictionary<Type,ComponentParameters> typeParameters = new Dictionary<Type,ComponentParameters>();

		protected override void Init()
		{
			typeParameters = new Dictionary<Type,ComponentParameters>();
			typeInstances = new Dictionary<Type,List<Component>>();
			exactTypeInstances = new Dictionary<Type,List<Component>>();

			foreach(var type in AssemblyCache.AllTypes.Where(t => !t.IsAbstract && typeof(Component).IsAssignableFrom(t))) {
				if(!typeParameters.TryGetValue(type,out var parameters)) {
					typeParameters[type] = parameters = new ComponentParameters();
				}

				var attributes = type.GetCustomAttributes<ComponentAttribute>();

				foreach(var attribute in attributes) {
					attribute.SetParameters(type,parameters);
				}
			}
		}

		public static ComponentParameters GetParameters(Type type) => Instance.typeParameters[type];
		//Count
		public static int CountComponents(Type type,bool exactType = false)
			=> (exactType ? Instance.exactTypeInstances : Instance.typeInstances).TryGetValue(type,out var list) ? list.Count : 0;
		public static int CountComponents<T>(bool exactType = false)
			=> CountComponents(typeof(T),exactType);
		//Enumerate
		public static IEnumerable<Component> EnumerateComponents(Type type,bool exactType = false)
			=> (exactType ? Instance.exactTypeInstances : Instance.typeInstances).TryGetValue(type,out var list) ? list : Enumerable.Empty<Component>();
		public static IEnumerable<T> EnumerateComponents<T>(bool exactType = false) where T : Component
		{
			foreach(var component in EnumerateComponents(typeof(T),exactType)) {
				yield return (T)component;
			}
		}

		internal static void RegisterInstance(Type type,Component component)
		{
			void AddToList(Type key,Dictionary<Type,List<Component>> dictionary)
			{
				if(!dictionary.TryGetValue(key,out var list)) {
					dictionary[key] = list = new List<Component>();
				}

				list.Add(component);
			}

			AddToList(type,Instance.exactTypeInstances);

			//TODO: This could've been made faster.
			foreach(var baseType in ReflectionUtils.EnumerateBaseTypes(type,true,typeof(Component))) {
				AddToList(baseType,Instance.typeInstances);
			}
		}
		internal static void UnregisterInstance(Type type,Component component)
		{
			void RemoveFromList(Type key,Dictionary<Type,List<Component>> dictionary)
			{
				if(!dictionary.TryGetValue(key,out var list)) {
					return;
				}

				list.Remove(component);

				if(list.Count==0) {
					dictionary.Remove(type);
				}
			}

			RemoveFromList(type,Instance.typeInstances);

			//Same as above.
			foreach(var baseType in ReflectionUtils.EnumerateBaseTypes(type,true,typeof(Component))) {
				RemoveFromList(baseType,Instance.typeInstances);
			}
		}
	}
}
