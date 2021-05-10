namespace Dissonance.Engine.Physics
{
	public class Collision
	{
		public readonly Entity Entity;
		public readonly Rigidbody Rigidbody;
		public readonly ICollider Collider;
		public readonly ContactPoint[] Contacts;

		public Collision(Entity entity, Rigidbody rigidbody, ICollider collider, ContactPoint[] contacts)
		{
			Entity = entity;
			Rigidbody = rigidbody;
			Collider = collider;
			Contacts = contacts;
		}
	}
}
