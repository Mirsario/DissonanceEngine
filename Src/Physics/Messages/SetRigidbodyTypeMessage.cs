namespace Dissonance.Engine.Physics
{
	internal readonly struct SetRigidbodyTypeMessage
	{
		public readonly Entity Entity;
		public readonly RigidbodyType RigidbodyType;

		public SetRigidbodyTypeMessage(Entity entity, RigidbodyType rigidbodyType)
		{
			Entity = entity;
			RigidbodyType = rigidbodyType;
		}
	}
}
