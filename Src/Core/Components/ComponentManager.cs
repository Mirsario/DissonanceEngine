using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.Core.Components.Attributes;
using Dissonance.Engine.Core.Modules;

namespace Dissonance.Engine.Core.Components
{
	public class ComponentManager : EngineModule
	{
		internal static ComponentManager Instance => Game.Instance.GetModule<ComponentManager>(true);

		private Dictionary<Type,List<Component>> typeInstances = new Dictionary<Type,List<Component>>();
		private Dictionary<Type,ComponentParameters> typeParameters = new Dictionary<Type,ComponentParameters>();

		protected override void Init()
		{
			typeInstances = new Dictionary<Type,List<Component>>();
			typeParameters = new Dictionary<Type,ComponentParameters>();

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
		public static bool TryGetInstanceList(Type type,out IReadOnlyList<Component> list)
		{
			bool result = Instance.typeInstances.TryGetValue(type,out var originalList);

			list = originalList;

			return result;
		}

		internal static void RegisterInstance(Type type,Component component)
		{
			var typeInstances = Instance.typeInstances;

			if(!typeInstances.TryGetValue(type,out var list)) {
				typeInstances[type] = list = new List<Component>();
			}

			list.Add(component);
		}
		internal static void UnregisterInstance(Type type,Component component)
		{
			var typeInstances = Instance.typeInstances;

			if(!typeInstances.TryGetValue(type,out var list)) {
				return;
			}

			list.Remove(component);

			if(list.Count==0) {
				typeInstances.Remove(type);
			}
		}
	}
}
