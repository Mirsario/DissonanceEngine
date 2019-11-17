using System;
using System.Collections.Generic;
using BulletSharp;

namespace GameEngine.Physics
{
	public static class PhysicsEngine
	{
		internal static DbvtBroadphase broadphase;
		internal static DiscreteDynamicsWorld world;
		internal static CollisionDispatcher dispatcher;
		internal static CollisionConfiguration collisionConf;
		internal static List<Collider> collidersToUpdate;
		internal static List<CollisionShape> collisionShapes;
		internal static List<RigidbodyInternal> rigidbodies;

		public static List<RigidbodyBase> ActiveRigidbodies	{ get; private set; }
		public static Vector3 Gravity {
			get => world.Gravity;
			set => world.Gravity = value;
		}
		
		public static void Init()
		{
			broadphase = new DbvtBroadphase();
			collisionConf = new DefaultCollisionConfiguration();
			collisionShapes = new List<CollisionShape>();
			collidersToUpdate = new List<Collider>();
			rigidbodies = new List<RigidbodyInternal>();
			ActiveRigidbodies = new List<RigidbodyBase>();

			dispatcher = new CollisionDispatcher(collisionConf);

			world = new DiscreteDynamicsWorld(dispatcher,broadphase,null,collisionConf);
			world.SetInternalTickCallback(InternalTickCallback);

			Gravity = new Vector3(0f,-9.81f,0f);

			ManifoldPoint.ContactAdded += Callback_ContactAdded;
			PersistentManifold.ContactProcessed += Callback_ContactProcessed;
			PersistentManifold.ContactDestroyed += Callback_ContactDestroyed;
		}
		public static void UpdateFixed()
		{
			for(int i = 0;i<rigidbodies.Count;i++) {
				var rigidbody = rigidbodies[i];
				if(!rigidbody.enabled) {
					continue;
				}

				var transform = rigidbody.gameObject.transform;
				if(transform.updatePhysicsPosition || transform.updatePhysicsScale || transform.updatePhysicsRotation) {
					Matrix4x4 matrix = rigidbody.btRigidbody.WorldTransform;

					if(transform.updatePhysicsPosition) {
						matrix.SetTranslation(transform.Position);

						transform.updatePhysicsPosition = false;
					}

					if(transform.updatePhysicsScale && transform.updatePhysicsRotation) {
						matrix.SetRotationAndScale(transform.Rotation,transform.LocalScale);
						transform.updatePhysicsScale = false;
						transform.updatePhysicsRotation = false;
					}else if(transform.updatePhysicsScale) {
						matrix.SetScale(transform.LocalScale);
						transform.updatePhysicsScale = false;
					}else if(transform.updatePhysicsRotation) {
						matrix.SetRotation(transform.Rotation);
						transform.updatePhysicsRotation = false;
					}

					rigidbody.btRigidbody.WorldTransform = matrix;
				}
			}

			world.StepSimulation(Time.fixedDeltaTime);
		}
		public static void UpdateRender()
		{
			//world.StepSimulation(Time.renderDeltaTime,1,0f);
			//fixedStep = false;
		}
		public static bool Raycast(Vector3 origin,Vector3 direction,out RaycastHit hit,float range = 100000f,Func<ulong,ulong> mask = null,Func<GameObject,bool?> customFilter = null,bool debug = false)
		{
			direction.Normalize();

			ulong layerMask = ulong.MaxValue;
			if(mask!=null) {
				layerMask = mask(layerMask);
			}

			BulletSharp.Vector3 rayEnd = origin+direction*range;
			BulletSharp.Vector3 origin2 = origin;
			var callback = new RaycastCallback(ref origin2,ref rayEnd,layerMask,customFilter);
			world.RayTest(origin,rayEnd,callback);

			if(!callback.HasHit) {
				hit = new RaycastHit {
					triangleIndex = -1,
				};
				return false;
			}
			hit = new RaycastHit {
				point = callback.HitPointWorld,
				triangleIndex = callback.triangleIndex,
				collider = callback.collider,
				gameObject = callback.collider?.gameObject
			};
			return true;
		}
		public static void Dispose()
		{
			world?.Dispose();
			dispatcher?.Dispose();
			broadphase?.Dispose();
			collidersToUpdate?.Clear();

			if(collisionShapes!=null) {
				for(int i = 0;i<collisionShapes.Count;i++) {
					collisionShapes[i].Dispose();
				}
			}

			if(rigidbodies!=null) {
				for(int i = 0;i<rigidbodies.Count;i++) {
					rigidbodies[i].Dispose();
				}
				rigidbodies.Clear();
			}
		}
		internal static CollisionShape GetSubShape(CollisionShape shape,int subIndex)
		{
			if(shape is CompoundShape compoundShape && compoundShape.NumChildShapes>0) {
				return compoundShape.GetChildShape(subIndex>=0 ? subIndex : 0);
			}
			return shape;
		}

		#region Callbacks
		private static void Callback_ContactAdded(ManifoldPoint cp,CollisionObjectWrapper colObj0,int partId0,int index0,CollisionObjectWrapper colObj1,int partId1,int index1)
		{
			//Bullet seems to use edge normals by default. Code below corrects it so it uses face normals instead.
			//This fixes tons of issues with rigidbodies jumping up when moving between terrain quads,even if terrain is 100% flat.
			var shape0 = colObj0.CollisionShape;
			var shape1 = colObj1.CollisionShape;
			var obj = shape0.ShapeType==BroadphaseNativeType.TriangleShape ? colObj0 : shape1.ShapeType==BroadphaseNativeType.TriangleShape ? colObj1 : null;
			if(obj!=null) {
				Matrix4x4 transform = obj.WorldTransform;
				transform.ClearTranslation();
				var shape = (TriangleShape)obj.CollisionShape;
				cp.NormalWorldOnB = (transform*Vector3.Cross(shape.Vertices[1]-shape.Vertices[0],shape.Vertices[2]-shape.Vertices[0])).Normalized;
			}
			//Debug.Log("Added Contact between "+Rand.Range(0,100));

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
					throw new Exception("UserObject wasn't a '"+typeof(RigidbodyInternal).FullName+"'.");
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
								point = doingA ? cPoint.PositionWorldOnB : cPoint.PositionWorldOnA,	//Should ContactPoint have two pairs of vectors?
								normal = cPoint.NormalWorldOnB,
								separation = cPoint.Distance,
							};
						}

						var collision = new Collision(otherRB.gameObject,rigidbody,null,contacts);
						thisRB.collisions.Add(collision);
					}else if(thisRB.rigidbody is Rigidbody2D rigidbody2D) {
						var contacts = new ContactPoint2D[numContacts];
						for(int k = 0;k<numContacts;k++) {
							var cPoint = contactManifold.GetContactPoint(k);
							contacts[k] = new ContactPoint2D {
								point = doingA ? ((Vector3)cPoint.PositionWorldOnB).XY : ((Vector3)cPoint.PositionWorldOnA).XY,	//Should ContactPoint have two pairs of vectors?
								normal = ((Vector3)cPoint.NormalWorldOnB).XY,
								separation = cPoint.Distance,
							};
						}

						var collision = new Collision2D(otherRB.gameObject,rigidbody2D,null,contacts);
						thisRB.collisions2D.Add(collision);
					}
				}
			}
		}
		#endregion
	}
	internal class RaycastCallback : ClosestRayResultCallback
	{
		public Func<GameObject,bool?> customFilter;
		public int triangleIndex = -1;
		public ulong layerMask;
		public Collider collider;

		public RaycastCallback(ref BulletSharp.Vector3 rayFromWorld,ref BulletSharp.Vector3 rayToWorld,ulong layerMask,Func<GameObject,bool?> customFilter) : base(ref rayFromWorld,ref rayToWorld)
		{
			this.layerMask = layerMask;
			this.customFilter = customFilter;
		}
		public override float AddSingleResult(LocalRayResult rayResult,bool normalInWorldSpace)
		{
			try {
				var rb = rayResult.CollisionObject;
				var shapeInfo = rayResult.LocalShapeInfo;
				if(rb!=null && shapeInfo!=null) {
					//Debug.Log(shapeInfo.ShapePart);
					var collShape = PhysicsEngine.GetSubShape(rb.CollisionShape,shapeInfo.ShapePart);
					if(collShape!=null) {
						var userObject = collShape.UserObject;
						if(userObject!=null && userObject is Collider coll) {
							collider = coll;
						}
						triangleIndex = shapeInfo.TriangleIndex;
					}
				}
			}
			catch {}
			return base.AddSingleResult(rayResult,normalInWorldSpace);
		}
		public override bool NeedsCollision(BroadphaseProxy proxy)
		{
			if(proxy.ClientObject is RigidBody bulletBody) {
				var rbInternal = bulletBody.UserObject as RigidbodyInternal;
				ulong objLayerMask = Layers.GetLayerMask(rbInternal.gameObject.layer);
				if(rbInternal!=null) {
					var resultOverride = customFilter?.Invoke(rbInternal.gameObject);
					if(resultOverride!=null) {
						return resultOverride.Value;
					}
					if((objLayerMask & layerMask)==0) {
						return false;
					}
				}
			}
			return base.NeedsCollision(proxy);
		}
	}
}

