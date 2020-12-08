using System;

namespace Dissonance.Engine
{
	public class AllowOnlyOneInWorld : ComponentAttribute
	{
		public override void SetParameters(Type type, ComponentParameters parameters) => parameters.allowOnlyOneInWorld = true;
	}
}
