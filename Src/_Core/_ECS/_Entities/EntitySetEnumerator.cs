using System.Collections.Generic;

namespace Dissonance.Engine;

public ref struct EntitySetEnumerator
{
	private readonly List<Entity> Entities;

	private int i;
	private Entity current;

	public Entity Current => current;

	public EntitySetEnumerator(List<Entity> entities)
	{
		Entities = entities;

		i = -1;
		current = default;
	}

	public bool MoveNext()
	{
		if (++i < Entities.Count) {
			current = Entities[i];

			return true;
		}

		current = default;

		return false;
	}

	public EntitySetEnumerator GetEnumerator() => this;
}
