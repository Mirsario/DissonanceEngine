using System;

namespace Dissonance.Engine
{
	public class AllowOnlyOnePerObjectAttribute : ComponentAttribute
	{
		public override void PreAddComponent(Entity entity, Type type)
		{
			//TODO:
			/*if(gameObject.CountComponents(type) >= 1) {
				throw new Exception($"Cannot add more than 1 component of type '{type.Name}' to a single {nameof(GameObject)}.");
			}*/
		}
	}
}
