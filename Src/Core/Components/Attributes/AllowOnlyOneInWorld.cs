using System;

namespace Dissonance.Engine
{
	public class AllowOnlyOneInWorld : ComponentAttribute
	{
		public override void OnComponentEnabled(GameObject gameObject, Component component)
		{
			if(ComponentManager.CountComponents(component.GetType()) >= 1) {
				throw new InvalidOperationException($"Attempted to enable a second instance of component '{GetType().Name}', but only 1 instance is allowed to be enabled at the same time.");
			}
		}
	}
}
