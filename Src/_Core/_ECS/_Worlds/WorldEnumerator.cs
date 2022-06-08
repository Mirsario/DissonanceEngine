using System.Collections.Generic;

namespace Dissonance.Engine;

public ref struct WorldEnumerator
{
	private readonly List<World> worlds;

	private int i;

	public World Current { get; private set; }

	public WorldEnumerator(List<World> worlds, int start)
	{
		this.worlds = worlds;

		i = start - 1;
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

	public WorldEnumerator GetEnumerator() => this;
}
