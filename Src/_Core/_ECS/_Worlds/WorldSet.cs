using System.Collections.Generic;

namespace Dissonance.Engine;

public sealed class WorldSet
{
	private readonly List<World> worlds = new();

	internal readonly ComponentSet componentSet;

	internal WorldSet(ComponentSet componentSet)
	{
		this.componentSet = componentSet;
	}

	public WorldSetEnumerator ReadWorlds()
	{
		return new WorldSetEnumerator(worlds);
	}

	internal void OnWorldUpdated(World world)
	{
		int index = worlds.IndexOf(world);
		bool contains = index != -1;
		bool shouldContain = componentSet.GetHashCode() == 0
			? world.Options.AddDefaultSystems
			: componentSet.Matches(world);

		if (contains != shouldContain) {
			if (shouldContain) {
				worlds.Add(world);
			} else {
				worlds.RemoveAt(index);
			}
		}
	}
}
