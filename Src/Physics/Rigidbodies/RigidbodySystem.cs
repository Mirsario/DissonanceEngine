using System;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Reads(typeof(Rigidbody))]
	[Writes(typeof(Rigidbody))]
	[Sends(typeof(ActivateRigidbodyMessage))]
	[Receives(typeof(AddCollisionShapeMessage), typeof(RemoveCollisionShapeMessage), typeof(ActivateRigidbodyMessage))]
	public sealed class RigidbodySystem : GameSystem
	{
		private EntitySet entities;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<Rigidbody>() && e.Has<Transform>());
		}

		protected internal override void FixedUpdate()
		{
			if(!World.Has<WorldPhysics>()) {
				return;
			}

			var physics = World.Get<WorldPhysics>();

			// Set UpdateShapes to true whenever collision shapes have been modified.

			foreach(var message in ReadMessages<RemoveCollisionShapeMessage>()) {
				if(message.Entity.Has<Rigidbody>()) {
					message.Entity.Get<Rigidbody>().updateShapes = true;
				}
			}

			foreach(var message in ReadMessages<AddCollisionShapeMessage>()) {
				if(message.Entity.Has<Rigidbody>()) {
					message.Entity.Get<Rigidbody>().updateShapes = true;
				}
			}

			// Update rigidbodies

			foreach(var entity in entities.ReadEntities()) {
				ref var rb = ref entity.Get<Rigidbody>();
				var collisionShapeData = entity.Has<CollisionShapesInfo>() ? entity.Get<CollisionShapesInfo>().CollisionShapes : null;

				rb.motionState ??= new RigidbodyMotionState(entity);

				if(rb.bulletRigidbody == null) {
					var collisionShape = new EmptyShape();

					using var rbInfo = new RigidBodyConstructionInfo(0f, rb.motionState, collisionShape, Vector3.Zero) {
						LinearSleepingThreshold = 0.1f,
						AngularSleepingThreshold = 1f,
						Friction = 0.6f,
						RollingFriction = 0f,
						Restitution = 0.1f
					};

					rb.bulletRigidbody = new RigidBody(rbInfo) {
						UserObject = entity,
						CollisionFlags = CollisionFlags.None
					};

					UpdateCollisionFlags(ref rb);
					UpdateShapes(entity, ref rb, collisionShapeData);

					physics.PhysicsWorld.AddRigidBody(rb.bulletRigidbody);
				}

				if(rb.updateFlags) {
					UpdateCollisionFlags(ref rb);
				}

				if(rb.updateMass) {
					UpdateMass(ref rb);
				}

				if(rb.updateShapes) {
					physics.PhysicsWorld.RemoveRigidBody(rb.bulletRigidbody);

					UpdateShapes(entity, ref rb, collisionShapeData);

					physics.PhysicsWorld.AddRigidBody(rb.bulletRigidbody);
				}
			}

			foreach(var message in ReadMessages<ActivateRigidbodyMessage>()) {
				if(message.Entity.Has<Rigidbody>()) {
					message.Entity.Get<Rigidbody>().bulletRigidbody.Activate();
				}
			}
		}

		private void UpdateShapes(in Entity entity, ref Rigidbody rb, ReadOnlySpan<CollisionShape> collisionShapes)
		{
			var previousShape = rb.bulletRigidbody.CollisionShape;

			CollisionShape resultShape;

			if(collisionShapes.Length == 0) {
				//EmptyShape
				resultShape = new EmptyShape();
			} else {
				//CompoundShape
				var compoundShape = new CompoundShape();

				for(int i = 0; i < collisionShapes.Length; i++) {
					compoundShape.AddChildShape(Matrix4x4.Identity, collisionShapes[i]);
				}

				resultShape = compoundShape;
			}

			rb.bulletRigidbody.CollisionShape = resultShape;

			if(rb.ownsCollisionShape && previousShape != null) {
				previousShape.Dispose();
			}

			rb.updateShapes = false;
			rb.ownsCollisionShape = true;

			SendMessage(new ActivateRigidbodyMessage(entity));
		}

		private static void UpdateCollisionFlags(ref Rigidbody rb)
		{
			var flags = rb.bulletRigidbody.CollisionFlags;

			void SetFlag(CollisionFlags flag, bool value)
			{
				if(value) {
					flags |= flag;
				} else {
					flags &= ~flag;
				}
			}

			SetFlag(CollisionFlags.StaticObject, rb.Type == RigidbodyType.Static);
			SetFlag(CollisionFlags.KinematicObject, rb.Type == RigidbodyType.Kinematic);

			rb.bulletRigidbody.CollisionFlags = flags;
			rb.updateShapes = false;
		}

		private static void UpdateMass(ref Rigidbody rb)
		{
			float realMass = rb.Type == RigidbodyType.Dynamic ? rb.Mass : 0f;
			var localInertia = realMass > 0f ? rb.bulletRigidbody.CollisionShape.CalculateLocalInertia(rb.Mass) : BulletSharp.Math.Vector3.Zero;

			rb.bulletRigidbody.SetMassProps(realMass, localInertia);

			rb.updateMass = false;
		}
	}
}
