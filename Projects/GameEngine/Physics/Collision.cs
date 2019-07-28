namespace GameEngine
{
	public struct ContactPoint
	{
		public Vector3 point;
		public Vector3 normal;
		public Collider thisCollider;
		public Collider otherCollider;
		public float separation;
	}
	public class Collision
	{
		public readonly GameObject GameObject;
		public readonly Rigidbody Rigidbody;
		public readonly Collider Collider;
		public readonly ContactPoint[] Contacts;

		public Collision(GameObject gameObject,Rigidbody rigidbody,Collider collider,ContactPoint[] contacts)
		{
			GameObject = gameObject;
			Rigidbody = rigidbody;
			Collider = collider;
			Contacts = contacts;
		}
	}
}
