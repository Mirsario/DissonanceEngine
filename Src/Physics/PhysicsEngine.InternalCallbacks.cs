using System;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	partial class PhysicsEngine
	{
		private static void Callback_ContactAdded(ManifoldPoint cp,CollisionObjectWrapper colObj0,int partId0,int index0,CollisionObjectWrapper colObj1,int partId1,int index1)
		{
			//Bullet seems to use edge normals by default. Code below corrects it so it uses face normals instead.
			//This fixes tons of issues with rigidbodies jumping up when moving between terrain quads, even if the terrain is 100% flat.

			var shape0 = colObj0.CollisionShape;
			var shape1 = colObj1.Parent.CollisionShape; //The need for .Parent here seems like a BulletSharp bug.
			var obj = shape0.ShapeType==BroadphaseNativeType.TriangleShape ? colObj0 : shape1.ShapeType==BroadphaseNativeType.TriangleShape ? colObj1 : null;

			if(obj!=null) {
				Matrix4x4 transform = obj.WorldTransform;

				transform.ClearTranslation();

				var shape = (TriangleShape)obj.CollisionShape;

				cp.NormalWorldOnB = (transform*Vector3.Cross(shape.Vertices[1]-shape.Vertices[0],shape.Vertices[2]-shape.Vertices[0])).Normalized;
			}

			//cp.UserPersistentData = colObj1Wrap.CollisionObject.UserObject;
		}
		private static void Callback_ContactProcessed(ManifoldPoint cp,CollisionObject body0,CollisionObject body1)
		{
			var rigidbodyA = (RigidbodyInternal)body0.UserObject;
			var rigidbodyB = (RigidbodyInternal)body1.UserObject;

			rigidbodyA.AddCollision(rigidbodyB);
			rigidbodyB.AddCollision(rigidbodyA);
			
			//Debug.Log("Processed Contact. "+Rand.Range(0,100));
			//cp.UserPersistentData = body0.UserObject;
		}
		private static void Callback_ContactDestroyed(object userPersistantData)
		{
			Debug.Log("Contact destroyed. "+Rand.Range(0,100));
		}
		internal static void InternalTickCallback(DynamicsWorld world,float timeStep)
		{
			var worldDispatcher = world.Dispatcher;
			int numManifolds = worldDispatcher.NumManifolds;
			
			for(int i = 0;i<rigidbodies.Count;i++) {
				rigidbodies[i].collisions.Clear();
			}

			for(int i = 0;i<numManifolds;i++) {
				var contactManifold = worldDispatcher.GetManifoldByIndexInternal(i);
				int numContacts = contactManifold.NumContacts;

				if(numContacts==0) {
					continue;
				}

				var objA = contactManifold.Body0;
				var objB = contactManifold.Body1;

				if(!(objA.UserObject is RigidbodyInternal rigidBodyA) || !(objB.UserObject is RigidbodyInternal rigidbodyB)) {
					throw new Exception($"UserObject wasn't a '{typeof(RigidbodyInternal).FullName}'.");
				}

				for(int j = 0;j<2;j++) {
					bool doingA = j==0;
					var thisRB = doingA ? rigidBodyA : rigidbodyB;
					var otherRB = doingA ? rigidbodyB : rigidBodyA;

					if(thisRB.rigidbody is Rigidbody rigidbody) {
						var contacts = new ContactPoint[numContacts];
						
						for(int k = 0;k<numContacts;k++) {
							var cPoint = contactManifold.GetContactPoint(k);

							contacts[k] = new ContactPoint {
								point = (doingA ? cPoint.PositionWorldOnB : cPoint.PositionWorldOnA), //Should ContactPoint have two pairs of vectors?
								normal = cPoint.NormalWorldOnB,
								separation = cPoint.Distance,
							};
						}

						thisRB.collisions.Add(new Collision(otherRB.gameObject,rigidbody,null,contacts));
					}else if(thisRB.rigidbody is Rigidbody2D rigidbody2D) {
						var contacts = new ContactPoint2D[numContacts];

						for(int k = 0;k<numContacts;k++) {
							var cPoint = contactManifold.GetContactPoint(k);

							contacts[k] = new ContactPoint2D {
								point = ((Vector3)(doingA ? cPoint.PositionWorldOnB : cPoint.PositionWorldOnA)).XY,	//Should ContactPoint have two pairs of vectors?
								normal = ((Vector3)cPoint.NormalWorldOnB).XY,
								separation = cPoint.Distance,
							};
						}

						thisRB.collisions2D.Add(new Collision2D(otherRB.gameObject,rigidbody2D,null,contacts));
					}
				}
			}
		}
	}
}
