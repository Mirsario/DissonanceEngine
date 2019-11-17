using System;

namespace GameEngine
{
	public class AllowOnlyOneInWorld : ComponentAttribute
	{
		public override void SetParameters(Type type) => Component.typeParameters[type].allowOnlyOneInWorld = true;
	}
}