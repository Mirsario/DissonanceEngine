using System.Collections.Generic;

namespace Dissonance.Engine;

public ref struct EntityEnumerator
{
	private readonly List<int> EntityIds;
	private readonly int WorldId;

	private int i;
	private int length;
	private Entity current;

	public Entity Current => current;

	public EntityEnumerator(int worldId, List<int> entityIds)
	{
		WorldId = worldId;
		EntityIds = entityIds;

		i = -1;
		length = entityIds.Count;
		current = default;
	}

	public bool MoveNext()
	{
		if (++i < length || i < (length = EntityIds?.Count ?? 0)) {
			current = new Entity(EntityIds[i], WorldId);

			return true;
		}

		current = default;

		return false;
	}

	public EntityEnumerator GetEnumerator() => this;
}
