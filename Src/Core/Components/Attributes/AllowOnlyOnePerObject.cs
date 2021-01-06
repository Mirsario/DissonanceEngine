using System;

namespace Dissonance.Engine
{
	public class AllowOnlyOnePerObject : ComponentAttribute
	{
		public override void PreAddComponent(GameObject gameObject, Type type)
		{
			if(gameObject.CountComponents(type) >= 1) {
				throw new Exception($"Cannot add more than 1 component of type '{type.Name}' to a single {nameof(GameObject)}.");
			}
		}
	}
}
