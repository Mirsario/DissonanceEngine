namespace Dissonance.Engine.Physics
{
	public class Collision2D
	{
		public readonly Entity Entity;
		public readonly Rigidbody2D Rigidbody;
		public readonly ICollider Collider;
		public readonly ContactPoint2D[] Contacts;

		public Collision2D(Entity entity, Rigidbody2D rigidbody, ICollider collider, ContactPoint2D[] contacts)
		{
			Entity = entity;
			Rigidbody = rigidbody;
			Collider = collider;
			Contacts = contacts;
		}
	}
}
