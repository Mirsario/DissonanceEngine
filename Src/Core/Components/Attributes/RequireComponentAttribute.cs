using System;
using System.Linq;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public class RequireComponentAttribute : ComponentAttribute
	{
		public readonly Type RequiredType;

		public RequireComponentAttribute(Type componentType)
		{
			AssertionUtils.TypeIsAssignableFrom(typeof(Component), componentType);

			RequiredType = componentType;
		}

		public override void PreAddComponent(GameObject gameObject, Type type)
		{
			if(!gameObject.Components.Any(c => c.GetType().IsAssignableFrom(RequiredType))) {
				throw new InvalidOperationException($"Expected '{RequiredType.Name}' component (required by '{type.Name}') was not found on '{gameObject.Name}' GameObject.");
			}
		}
	}
}
