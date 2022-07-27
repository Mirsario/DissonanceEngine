using System;

namespace Dissonance.Engine.IO;

[AttributeUsage(AttributeTargets.Class)]
public class AutoloadRequirement : Attribute
{
	public readonly Type[] Requirements;

	public AutoloadRequirement(params Type[] types)
	{
		if (types == null || types.Length == 0) {
			throw new ArgumentException($"'{nameof(types)}' array cannot be null or empty");
		}

		Requirements = types;
	}
}
