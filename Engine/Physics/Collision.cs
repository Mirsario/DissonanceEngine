using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public readonly GameObject gameObject;
		public readonly Rigidbody rigidbody;
		public readonly Collider collider;
		public ContactPoint[] contacts;

		public Collision(GameObject gameObject,Rigidbody rigidbody,Collider collider,ContactPoint[] contacts)
		{
			this.gameObject = gameObject;
			this.rigidbody = rigidbody;
			this.collider = collider;
			this.contacts = contacts;
		}
	}
}
