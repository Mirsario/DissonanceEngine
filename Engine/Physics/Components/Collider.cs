using System;
using System.Collections.Generic;
using BulletSharp;

namespace GameEngine
{
	public abstract class Collider : PhysicsComponent
	{
		internal CollisionShape collShape;
		public Vector3 offset = Vector3.zero;
		
		internal virtual void UpdateCollider()
		{
			if(collShape!=null) {
				collShape.UserObject = this;
			}
			gameObject.rigidbodyInternal.UpdateShape();
		}
	}
}

