using System;
using ComponentInstanceLists = Dissonance.Engine.InstanceLists<Dissonance.Engine.Component>;

namespace Dissonance.Engine
{
	internal class ComponentTypeInfo
	{
		public Type type;
		public ComponentInstanceLists sharedInstances = new ComponentInstanceLists();
		public ComponentInstanceLists exactInstances = new ComponentInstanceLists();

		public ComponentTypeInfo(Type type)
		{
			this.type = type;
		}
	}
}
