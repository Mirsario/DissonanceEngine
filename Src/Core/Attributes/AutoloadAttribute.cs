using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dissonance.Engine.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Class,AllowMultiple = true,Inherited = true)]
	public class AutoloadAttribute : Attribute
	{
		private static readonly AutoloadAttribute Default = new AutoloadAttribute();

		public readonly bool Enabled;

		public GameFlags RequiredGameFlags { get; set; } = GameFlags.None;

		public bool NeedsAutoloading => Enabled && (Game.Instance.Flags & RequiredGameFlags)>=RequiredGameFlags;

		public AutoloadAttribute(bool enabled = true)
		{
			Enabled = enabled;
		}

		public static AutoloadAttribute Get(Type type)
			=> (AutoloadAttribute)type.GetCustomAttributes(typeof(AutoloadAttribute),true).FirstOrDefault() ?? Default;
	}
}
