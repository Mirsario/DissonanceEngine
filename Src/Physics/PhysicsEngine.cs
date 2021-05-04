using BulletSharp;
using System;
using System.Collections.Generic;

namespace Dissonance.Engine.Physics
{
	public sealed partial class PhysicsEngine : EngineModule
	{
		internal static DbvtBroadphase broadphase;
		internal static DiscreteDynamicsWorld world;
		internal static CollisionDispatcher dispatcher;
		internal static CollisionConfiguration collisionConf;
		internal static List<Collider> collidersToUpdate;
		internal static List<CollisionShape> collisionShapes;
		internal static List<RigidbodyInternal> rigidbodies;

		public static List<RigidbodyBase> ActiveRigidbodies { get; private set; }
		public static Vector3 Gravity {
			get => world.Gravity;
			set => world.Gravity = value;
		}

		protected override void Init()
		{
			collisionShapes = new List<CollisionShape>();
			collidersToUpdate = new List<Collider>();
			rigidbodies = new List<RigidbodyInternal>();
			ActiveRigidbodies = new List<RigidbodyBase>();

			collisionConf = new DefaultCollisionConfiguration();
			broadphase = new DbvtBroadphase();
			dispatcher = new CollisionDispatcher(collisionConf);
			world = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConf);

			world.SetInternalTickCallback(InternalTickCallback);

			Gravity = new Vector3(0f, -9.81f, 0f);

			ManifoldPoint.ContactAdded += Callback_ContactAdded;
			PersistentManifold.ContactProcessed += Callback_ContactProcessed;
			PersistentManifold.ContactDestroyed += Callback_ContactDestroyed;
		}
		protected override void PostFixedUpdate()
		{
			for(int i = 0; i < rigidbodies.Count; i++) {
				var rigidbody = rigidbodies[i];

				if(rigidbody.enabled) {
					rigidbody.MotionState.Update();
				}
			}

			world.StepSimulation(Time.FixedDeltaTime);
		}
		protected override void PostRenderUpdate()
		{
			//world.StepSimulation(Time.renderDeltaTime,1,0f);
			//fixedStep = false;
		}

		public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hit, float range = 100000f, Func<ulong, ulong> mask = null, Func<GameObject, bool?> customFilter = null, bool debug = false)
		{
			direction.Normalize();

			ulong layerMask = ulong.MaxValue;

			if(mask != null) {
				layerMask = mask(layerMask);
			}

			BulletSharp.Math.Vector3 rayEnd = (origin + direction * range);
			BulletSharp.Math.Vector3 origin2 = origin;

			var callback = new RaycastCallback(ref origin2, ref rayEnd, layerMask, customFilter);

			world.RayTest(origin, rayEnd, callback);

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
		protected override void OnDispose()
		{
			world?.Dispose();
			dispatcher?.Dispose();
			broadphase?.Dispose();
			collidersToUpdate?.Clear();

			if(collisionShapes != null) {
				for(int i = 0; i < collisionShapes.Count; i++) {
					collisionShapes[i].Dispose();
				}
			}

			if(rigidbodies != null) {
				for(int i = 0; i < rigidbodies.Count; i++) {
					rigidbodies[i].Dispose();
				}

				rigidbodies.Clear();
			}
		}
		internal static CollisionShape GetSubShape(CollisionShape shape, int subIndex)
		{
			if(shape is CompoundShape compoundShape && compoundShape.NumChildShapes > 0) {
				return compoundShape.GetChildShape(subIndex >= 0 ? subIndex : 0);
			}

			return shape;
		}
	}
}
