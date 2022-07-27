namespace Dissonance.Engine;

public readonly struct ComponentRemovedMessage<T> where T : struct
{
	public readonly Entity Entity;
	public readonly T Value;

	internal ComponentRemovedMessage(Entity entity, T value)
	{
		Entity = entity;
		Value = value;
	}
}
