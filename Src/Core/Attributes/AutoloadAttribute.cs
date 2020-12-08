using System;
using System.Linq;

namespace Dissonance.Engine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class AutoloadAttribute : Attribute
	{
		private static readonly AutoloadAttribute Default = new AutoloadAttribute();

		public readonly bool Enabled;

		public GameFlags RequiredGameFlags { get; set; } = GameFlags.None;
		public GameFlags DisablingGameFlags { get; set; } = GameFlags.None;

		public bool NeedsAutoloading => Enabled && (Game.Instance.Flags & RequiredGameFlags) >= RequiredGameFlags && (Game.Instance.Flags & DisablingGameFlags) == 0;

		public AutoloadAttribute(bool enabled = true)
		{
			Enabled = enabled;
		}

		public static AutoloadAttribute Get(Type type)
			=> (AutoloadAttribute)type.GetCustomAttributes(typeof(AutoloadAttribute), true).FirstOrDefault() ?? Default;
	}
}
