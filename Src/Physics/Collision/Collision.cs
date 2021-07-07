namespace Dissonance.Engine.Physics
{
	public class Collision
	{
		public readonly Entity Entity;
		public readonly ICollider Collider;
		public readonly ContactPoint[] Contacts;

		public Collision(Entity entity, ICollider collider, ContactPoint[] contacts)
		{
			Entity = entity;
			Collider = collider;
			Contacts = contacts;
		}
	}
}
