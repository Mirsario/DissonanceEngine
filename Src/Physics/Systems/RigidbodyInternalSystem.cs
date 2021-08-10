using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Reads(typeof(RigidbodyInternal))]
	[Writes(typeof(RigidbodyInternal))]
	[Sends(typeof(ActivateRigidbodyMessage))]
	[Receives(
		typeof(CreateInternalRigidbodyMessage),
		typeof(SetRigidbodyMassMessage),
		typeof(AddCollisionShapeMessage),
		typeof(RemoveCollisionShapeMessage),
		typeof(ActivateRigidbodyMessage)
	)]
	public sealed class RigidbodyInternalSystem : GameSystem
	{
		private EntitySet entities;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<RigidbodyInternal>() && e.Has<Transform>());
		}

		protected internal override void FixedUpdate()
		{
			if(!World.Has<WorldPhysics>()) {
				return;
			}

			var physics = World.Get<WorldPhysics>();

			static void EnsureInternalRigidbodyExists(Entity entity)
			{
				if(!entity.Has<RigidbodyInternal>()) {
					entity.Set(RigidbodyInternal.Create());
				}
			}

			foreach(var message in ReadMessages<CreateInternalRigidbodyMessage>()) {
				EnsureInternalRigidbodyExists(message.Entity);
			}

			foreach(var message in ReadMessages<RemoveCollisionShapeMessage>()) {
				if(!message.Entity.Has<RigidbodyInternal>()) {
					continue;
				}

				ref var rb = ref message.Entity.Get<RigidbodyInternal>();

				rb.CollisionShapes.Remove(message.CollisionShape);

				rb.updateShapes = true;
			}

			foreach(var message in ReadMessages<AddCollisionShapeMessage>()) {
				EnsureInternalRigidbodyExists(message.Entity);
				message.Entity.Get<RigidbodyInternal>().CollisionShapes.Add(message.CollisionShape);
			}

			foreach(var entity in entities.ReadEntities()) {
				ref var rb = ref entity.Get<RigidbodyInternal>();

				rb.MotionState ??= new RigidbodyMotionState(entity);
				rb.CollisionShape ??= new EmptyShape();

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
						CollisionFlags = CollisionFlags.None
					};

					UpdateShapes(entity, ref rb);

					physics.PhysicsWorld.AddRigidBody(rb.BulletRigidbody);
				}

				if(rb.updateShapes) {
					physics.PhysicsWorld.RemoveRigidBody(rb.BulletRigidbody);

					UpdateShapes(entity, ref rb);

					physics.PhysicsWorld.AddRigidBody(rb.BulletRigidbody);
				}
			}

			foreach(var message in ReadMessages<ActivateRigidbodyMessage>()) {
				if(message.Entity.Has<RigidbodyInternal>()) {
					message.Entity.Get<RigidbodyInternal>().BulletRigidbody.Activate();
				}
			}

			foreach(var message in ReadMessages<SetRigidbodyMassMessage>()) {
				ref var rb = ref message.Entity.Get<RigidbodyInternal>();

				float realMass = rb.rigidbodyType == RigidbodyType.Dynamic ? message.Mass : 0f;
				var localInertia = realMass > 0f ? rb.BulletRigidbody.CollisionShape.CalculateLocalInertia(message.Mass) : BulletSharp.Math.Vector3.Zero;

				rb.BulletRigidbody.SetMassProps(realMass, localInertia);
			}
		}

		private void UpdateShapes(in Entity entity, ref RigidbodyInternal rb)
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

			SendMessage(new ActivateRigidbodyMessage(entity));
		}
	}
}
