using System;

namespace Dissonance.Engine.Core.Components.Attributes
{
	public class AllowOnlyOnePerObject : ComponentAttribute
	{
		public override void SetParameters(Type type) => ComponentManager.typeParameters[type].allowOnlyOnePerObject = true;
	}
}