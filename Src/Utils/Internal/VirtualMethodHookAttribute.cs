using System;

namespace Dissonance.Engine.Utils
{
	internal class VirtualMethodHookAttribute : Attribute
	{
		public readonly Type HookHolder;
		public readonly string HookName;
		public readonly bool IsProperty;
		public readonly bool IsStatic;

		public VirtualMethodHookAttribute(Type hookHolder, string hookName, bool isStatic, bool isProperty)
		{
			HookHolder = hookHolder ?? throw new ArgumentNullException(nameof(hookHolder));
			HookName = hookName ?? throw new ArgumentNullException(nameof(hookName));
			IsStatic = isStatic;
			IsProperty = isProperty;
		}
	}
}
