using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dissonance.Engine;

public struct CallbackDescription
{
	internal readonly List<SystemHandle> systems = new();

	public readonly Type Type;

	internal bool NeedsSorting;

	public ReadOnlySpan<SystemHandle> Systems => CollectionsMarshal.AsSpan(systems);

	public CallbackDescription(Type type)
	{
		Type = type;
	}
}
