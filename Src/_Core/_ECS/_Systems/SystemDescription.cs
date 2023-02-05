using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Dissonance.Engine;

public readonly struct SystemDescription
{
	public readonly MethodInfo Method;
	public readonly SystemDelegate Function;

	internal readonly List<Tag> tags = new();
	internal readonly List<Tag> requiredTags = new();

	public ReadOnlySpan<Tag> Tags => CollectionsMarshal.AsSpan(tags);
	public ReadOnlySpan<Tag> RequiredTags => CollectionsMarshal.AsSpan(requiredTags);

	internal SystemDescription(MethodInfo method, SystemDelegate function)
	{
		Method = method;
		Function = function;
	}
}
