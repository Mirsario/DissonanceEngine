using System;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AutoloadAttribute : Attribute
{
	private static readonly AutoloadAttribute Default = new();

	public readonly bool Enabled;

	public GameFlags RequiredGameFlags { get; set; } = GameFlags.None;
	public GameFlags DisablingGameFlags { get; set; } = GameFlags.None;

	public bool NeedsAutoloading => Enabled && (GameEngine.Flags & RequiredGameFlags) >= RequiredGameFlags && (GameEngine.Flags & DisablingGameFlags) == 0;

	public AutoloadAttribute(bool enabled = true)
	{
		Enabled = enabled;
	}

	public static bool TypeNeedsAutoloading(Type type)
	{
		return (type.GetCustomAttribute<AutoloadAttribute>(true) ?? Default).NeedsAutoloading;
	}
}
