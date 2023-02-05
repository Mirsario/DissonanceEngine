using System;
using System.Collections.Concurrent;
using System.Numerics;

namespace Dissonance.Engine;

public static class Tags
{
	private struct TagData
	{
		public string Name;
		public SystemHandle[] Systems;
		public uint SystemsCount;
	}

	private static readonly ConcurrentDictionary<string, uint> tagIdsByString = new(StringComparer.InvariantCultureIgnoreCase);

	private static TagData[] tagDataById = new TagData[64];

	public static uint TagCount { get; private set; }

	public static Tag GetOrCreate(string name)
	{
		if (tagIdsByString.TryGetValue(name, out uint id)) {
			return new Tag(id);
		}

		TagData data;

		data.Name = name;
		data.Systems = GC.AllocateUninitializedArray<SystemHandle>(8);
		data.SystemsCount = 0;

		id = TagCount++;

		if (id >= tagDataById.Length) {
			Array.Resize(ref tagDataById, (int)BitOperations.RoundUpToPowerOf2(id + 1));
		}

		tagDataById[id] = data;
		tagIdsByString[name] = id;

		return new Tag(id);
	}

	public static ReadOnlySpan<SystemHandle> GetTagSystems(Tag tag)
	{
		ref readonly var data = ref tagDataById[tag.Id];

		return new ReadOnlySpan<SystemHandle>(data.Systems, 0, (int)data.SystemsCount);
	}

	internal static string GetName(uint id)
		=> tagDataById[id].Name;

	internal static void AddSystem(Tag tag, SystemHandle system)
	{
		ref var data = ref tagDataById[tag.Id];
		uint id = data.SystemsCount++;

		if (id >= data.Systems.Length) {
			Array.Resize(ref data.Systems, (int)BitOperations.RoundUpToPowerOf2(id + 1));
		}

		data.Systems[id] = system;
	}
}
