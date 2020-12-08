namespace Dissonance.Engine.Physics
{
	public class Collision
	{
		public readonly GameObject GameObject;
		public readonly Rigidbody Rigidbody;
		public readonly Collider Collider;
		public readonly ContactPoint[] Contacts;

		public Collision(GameObject gameObject, Rigidbody rigidbody, Collider collider, ContactPoint[] contacts)
		{
			GameObject = gameObject;
			Rigidbody = rigidbody;
			Collider = collider;
			Contacts = contacts;
		}
	}
}
