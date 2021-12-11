using System;

namespace Dissonance.Engine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class CallbackAttribute<T> : SystemTypeDataAttribute
		where T : CallbackSystem
	{
		public override void ModifySystemTypeData(SystemTypeData systemTypeData)
		{
			systemTypeData.Callbacks.Add(typeof(T));
		}
	}
}
