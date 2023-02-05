using System;
using static Dissonance.Engine.Tags;

namespace Dissonance.Engine;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequiresTagsAttribute : Attribute, ISystemAttribute
{
	public Tag[] Tags { get; }
	public bool Optional { get; init; }

	public RequiresTagsAttribute(params string[] tagNames)
	{
		int length = tagNames.Length;

		Tags = GC.AllocateUninitializedArray<Tag>(length);

		for (int i = 0; i < length; i++) {
			Tags[i] = GetOrCreate(tagNames[i]);
		}
	}

	void ISystemAttribute.ConfigureSystem(SystemHandle system)
		=> Systems.AddTagRequirementsToSystem(system, Tags);
}
