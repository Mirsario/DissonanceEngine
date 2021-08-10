namespace Dissonance.Engine.Physics
{
	public readonly struct ActivateRigidbodyMessage
	{
		public readonly Entity Entity;

		public ActivateRigidbodyMessage(Entity entity)
		{
			Entity = entity;
		}
	}
}
