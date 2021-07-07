using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Reads(typeof(RigidbodyInternal))]
	[Writes(typeof(RigidbodyInternal))]
	[Receives(typeof(AddCollisionShapeMessage), typeof(RemoveCollisionShapeMessage))]
	public sealed class InternalRigidbodySystem : GameSystem
	{
		private EntitySet entities;

		public override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<RigidbodyInternal>() && e.Has<Transform>());
		}

		public override void FixedUpdate()
		{
			if(!World.Has<WorldPhysics>()) {
				return;
			}

			var physics = World.Get<WorldPhysics>();

			foreach(var message in ReadMessages<RemoveCollisionShapeMessage>()) {
				if(!message.Entity.Has<RigidbodyInternal>()) {
					continue;
				}

				message.Entity.Get<RigidbodyInternal>().CollisionShapes.Remove(message.CollisionShape);
			}

			foreach(var message in ReadMessages<AddCollisionShapeMessage>()) {
				if(!message.Entity.Has<RigidbodyInternal>()) {
					message.Entity.Set(RigidbodyInternal.Create());
				}

				message.Entity.Get<RigidbodyInternal>().CollisionShapes.Add(message.CollisionShape);
			}

			foreach(var entity in entities.ReadEntities()) {
				ref var rb = ref entity.Get<RigidbodyInternal>();

				rb.MotionState ??= new RigidbodyMotionState(entity);
				rb.CollisionShape ??= new EmptyShape();

				static void UpdateShapes(ref RigidbodyInternal rb)
				{
					if(rb.ownsCollisionShape && rb.CollisionShape != null) {
						rb.CollisionShape.Dispose();
					}

					CollisionShape resultShape;

					var collisionShapes = rb.CollisionShapes;

					if(collisionShapes.Count == 0) {
						//EmptyShape
						resultShape = new EmptyShape();
					} else {
						//CompoundShape
						var compoundShape = new CompoundShape();

						for(int i = 0; i < collisionShapes.Count; i++) {
							compoundShape.AddChildShape(Matrix4x4.Identity, collisionShapes[i]);
						}

						resultShape = compoundShape;
					}

					rb.ownsCollisionShape = true;
					rb.BulletRigidbody.CollisionShape = resultShape;

					rb.updateShapes = false;
					rb.updateMass = true;
				}

				if(rb.BulletRigidbody == null) {
					var rbInfo = new RigidBodyConstructionInfo(0f, rb.MotionState, rb.CollisionShape, Vector3.Zero) {
						LinearSleepingThreshold = 0.1f,
						AngularSleepingThreshold = 1f,
						Friction = 0.6f,
						RollingFriction = 0f,
						Restitution = 0.1f
					};

					rb.BulletRigidbody = new RigidBody(rbInfo) {
						UserObject = entity,
					};

					UpdateShapes(ref rb);

					//rb.BulletRigidbody.CollisionFlags |= CollisionFlags.CustomMaterialCallback;

					physics.PhysicsWorld.AddRigidBody(rb.BulletRigidbody);
				}

				if(rb.updateShapes) {
					physics.PhysicsWorld.RemoveRigidBody(rb.BulletRigidbody);

					UpdateShapes(ref rb);

					physics.PhysicsWorld.AddRigidBody(rb.BulletRigidbody);
				}

				if(rb.updateMass) {
					float realMass = rb.rigidbodyType == RigidbodyType.Dynamic ? rb.Mass : 0f;
					var localInertia = realMass > 0f ? rb.BulletRigidbody.CollisionShape.CalculateLocalInertia(rb.Mass) : BulletSharp.Math.Vector3.Zero;

					rb.BulletRigidbody.SetMassProps(realMass, localInertia);

					rb.updateMass = false;
				}
			}
		}
	}
}
