namespace Dissonance.Engine.Physics
{
	public readonly struct CreateInternalRigidbodyMessage
	{
		public readonly Entity Entity;

		public CreateInternalRigidbodyMessage(Entity entity)
		{
			Entity = entity;
		}
	}
}
