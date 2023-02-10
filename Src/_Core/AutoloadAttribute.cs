using System.Reflection;
using static System.AttributeTargets;

namespace Dissonance.Engine;

[System.AttributeUsage(Class | Struct | Method | Delegate, AllowMultiple = true, Inherited = true)]
public class AutoloadAttribute : System.Attribute
{
	public readonly bool Enabled;

	public GameFlags RequiredGameFlags { get; set; } = GameFlags.None;
	public GameFlags DisablingGameFlags { get; set; } = GameFlags.None;

	public bool NeedsAutoloading
		=> Enabled && (GameEngine.Flags & RequiredGameFlags) >= RequiredGameFlags && (GameEngine.Flags & DisablingGameFlags) == 0;

	public AutoloadAttribute(bool enabled = true)
	{
		Enabled = enabled;
	}

	public static bool MemberNeedsAutoloading(MemberInfo member, bool defaultValue)
	{
		return (member.GetCustomAttribute<AutoloadAttribute>(true)?.NeedsAutoloading) ?? defaultValue;
	}
}
