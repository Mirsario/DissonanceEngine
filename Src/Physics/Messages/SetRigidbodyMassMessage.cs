namespace Dissonance.Engine.Physics
{
	internal readonly struct SetRigidbodyMassMessage
	{
		public readonly Entity Entity;
		public readonly float Mass;

		public SetRigidbodyMassMessage(Entity entity, float mass)
		{
			Entity = entity;
			Mass = mass;
		}
	}
}
