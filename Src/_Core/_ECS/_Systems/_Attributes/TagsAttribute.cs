using System;
using static Dissonance.Engine.Tags;

namespace Dissonance.Engine;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class TagsAttribute : Attribute, ISystemAttribute
{
	public Tag[] Tags { get; }

	public TagsAttribute(params string[] tagNames)
	{
		int length = tagNames.Length;

		Tags = GC.AllocateUninitializedArray<Tag>(length);

		for (int i = 0; i < length; i++) {
			Tags[i] = GetOrCreate(tagNames[i]);
		}
	}

	void ISystemAttribute.ConfigureSystem(SystemHandle system)
		=> Systems.AddTagsToSystem(system, Tags);
}
