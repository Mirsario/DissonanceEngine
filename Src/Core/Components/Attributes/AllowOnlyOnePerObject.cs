using System;

namespace Dissonance.Engine
{
	public class AllowOnlyOnePerObject : ComponentAttribute
	{
		public override void SetParameters(Type type, ComponentParameters parameters) => parameters.allowOnlyOnePerObject = true;
	}
}
