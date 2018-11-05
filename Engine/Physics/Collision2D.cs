using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public readonly GameObject gameObject;
		public readonly Rigidbody2D rigidbody;
		public readonly Collider collider;
		public ContactPoint2D[] contacts;

		public Collision2D(GameObject gameObject,Rigidbody2D rigidbody,Collider collider,ContactPoint2D[] contacts)
		{
			this.gameObject = gameObject;
			this.rigidbody = rigidbody;
			this.collider = collider;
			this.contacts = contacts;
		}
	}
}
