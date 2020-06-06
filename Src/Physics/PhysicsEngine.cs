using BulletSharp;
using System;
using System.Collections.Generic;

namespace Dissonance.Engine.Physics
{
	public static partial class PhysicsEngine
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
			collisionShapes = new List<CollisionShape>();
			collidersToUpdate = new List<Collider>();
			rigidbodies = new List<RigidbodyInternal>();
			ActiveRigidbodies = new List<RigidbodyBase>();

			collisionConf = new DefaultCollisionConfiguration();
			broadphase = new DbvtBroadphase();
			dispatcher = new CollisionDispatcher(collisionConf);
			world = new DiscreteDynamicsWorld(dispatcher,broadphase,null,collisionConf);

			world.SetInternalTickCallback(InternalTickCallback);

			Gravity = new Vector3(0f,-9.81f,0f);
			
			ManifoldPoint.ContactAdded += Callback_ContactAdded;
			PersistentManifold.ContactProcessed += Callback_ContactProcessed;
			PersistentManifold.ContactDestroyed += Callback_ContactDestroyed;
		}
		public static void FixedUpdate()
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
					} else if(transform.updatePhysicsScale) {
						matrix.SetScale(transform.LocalScale);
						transform.updatePhysicsScale = false;
					} else if(transform.updatePhysicsRotation) {
						matrix.SetRotation(transform.Rotation);
						transform.updatePhysicsRotation = false;
					}

					rigidbody.btRigidbody.WorldTransform = matrix;
				}
			}

			world.StepSimulation(Time.FixedDeltaTime);
		}
		public static void RenderUpdate()
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

			BulletSharp.Math.Vector3 rayEnd = (origin+direction*range);
			BulletSharp.Math.Vector3 origin2 = origin;

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
	}
}
