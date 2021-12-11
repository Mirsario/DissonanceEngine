using System;

namespace Dissonance.Engine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class ExecuteBeforeAttribute<T> : SystemTypeDataAttribute
		where T : GameSystem
	{
		public override void ModifySystemTypeData(SystemTypeData systemTypeData)
		{
			
		}
	}
}
