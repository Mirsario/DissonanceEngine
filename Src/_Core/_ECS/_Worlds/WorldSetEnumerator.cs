using System.Collections.Generic;

namespace Dissonance.Engine;

public ref struct WorldSetEnumerator
{
	private readonly List<World> worlds;

	private int i;

	public World Current { get; private set; }

	public WorldSetEnumerator(List<World> worlds)
	{
		this.worlds = worlds;

		i = -1;
		Current = default;
	}

	public bool MoveNext()
	{
		if (++i < worlds.Count) {
			Current = worlds[i];

			return true;
		}

		Current = default;

		return false;
	}

	public WorldSetEnumerator GetEnumerator() => this;
}
