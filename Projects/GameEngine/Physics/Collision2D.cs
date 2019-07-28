namespace GameEngine
{
	public struct ContactPoint2D
	{
		public Vector2 point;
		public Vector2 normal;
		public Collider thisCollider;
		public Collider otherCollider;
		public float separation;
	}

	public class Collision2D
	{
		public readonly GameObject GameObject;
		public readonly Rigidbody2D Rigidbody;
		public readonly Collider Collider;
		public readonly ContactPoint2D[] Contacts;

		public Collision2D(GameObject gameObject,Rigidbody2D rigidbody,Collider collider,ContactPoint2D[] contacts)
		{
			GameObject = gameObject;
			Rigidbody = rigidbody;
			Collider = collider;
			Contacts = contacts;
		}
	}
}
