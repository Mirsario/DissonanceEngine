using System;

namespace Dissonance.Engine.Physics;

public readonly struct Collision
{
	public readonly Entity Entity;

	private readonly ContactPoint[] contactPoints;

	public ReadOnlySpan<ContactPoint> ContactPoints => contactPoints;

	public Collision(Entity entity, ContactPoint[] contactPoints)
	{
		Entity = entity;
		this.contactPoints = contactPoints;
	}
}
