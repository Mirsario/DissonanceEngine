using System;

namespace GameEngine
{
	public class AllowOnlyOnePerObject : ComponentAttribute
	{
		public override void SetParameters(Type type) => Component.typeParameters[type].allowOnlyOnePerObject = true;
	}
}