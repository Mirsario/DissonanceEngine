using System;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class ModuleAutoloadAttribute : Attribute
	{
		private static readonly ModuleAutoloadAttribute Default = new ModuleAutoloadAttribute();

		public readonly bool Enabled;

		public GameFlags RequiredGameFlags { get; set; } = GameFlags.Default;
		public GameFlags DisablingGameFlags { get; set; } = GameFlags.Default;

		public bool NeedsAutoloading => Enabled && (Game.Instance.Flags & RequiredGameFlags) >= RequiredGameFlags && (Game.Instance.Flags & DisablingGameFlags) == 0;

		public ModuleAutoloadAttribute(bool enabled = true)
		{
			Enabled = enabled;
		}

		public static bool TypeNeedsAutoloading(Type type)
		{
			return (type.GetCustomAttribute<ModuleAutoloadAttribute>(true) ?? Default).NeedsAutoloading;
		}
	}
}
