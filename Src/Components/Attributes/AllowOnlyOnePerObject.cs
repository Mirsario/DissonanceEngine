using System;

namespace Dissonance.Engine
{
	public class AllowOnlyOnePerObject : ComponentAttribute
	{
		public override void SetParameters(Type type) => Component.typeParameters[type].allowOnlyOnePerObject = true;
	}
}