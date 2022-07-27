namespace Dissonance.Engine;

public readonly struct ComponentAddedMessage<T> where T : struct
{
	public readonly Entity Entity;
	public readonly T Value;

	internal ComponentAddedMessage(Entity entity, T value)
	{
		Entity = entity;
		Value = value;
	}
}
