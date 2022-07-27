using System;

namespace Dissonance.Engine;

public struct DependencyInfo
{
	public Type Type;
	public bool Optional;

	public DependencyInfo(Type type, bool optional = false)
	{
		Type = type;
		Optional = optional;
	}
}
