using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.Core.Components.Attributes;
using Dissonance.Engine.Core.Internal;
using Dissonance.Engine.Core.Modules;

namespace Dissonance.Engine.Core.Components
{
	public class ComponentManager : EngineModule
	{
		internal static Dictionary<Type,ComponentParameters> typeParameters = new Dictionary<Type,ComponentParameters>();
		internal static Dictionary<Type,List<Component>> typeInstances = new Dictionary<Type,List<Component>>();

		protected override void Init()
		{
			foreach(var type in AssemblyCache.AllTypes.Where(t => !t.IsAbstract && typeof(Component).IsAssignableFrom(t))) {
				if(!typeParameters.ContainsKey(type)) {
					typeParameters[type] = new ComponentParameters();
				}

				var attributes = type.GetCustomAttributes<ComponentAttribute>();

				foreach(var attribute in attributes) {
					attribute.SetParameters(type);
				}
			}
		}
	}
}
