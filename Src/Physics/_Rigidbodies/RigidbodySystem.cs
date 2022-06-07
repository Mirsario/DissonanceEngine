using System;
using BulletSharp;

namespace Dissonance.Engine.Physics;

[Callback<PhysicsUpdateGroup>]
[ExecuteAfter<ColliderUpdateGroup>]
[ExecuteAfter<CollisionShapesInfoSystem>]
public sealed partial class RigidbodySystem : GameSystem
{
	// Set UpdateShapes to true whenever collision shapes have been modified.

	[MessageSubsystem]
	partial void ReadCollisionShapeRemovalMessages(in RemoveCollisionShapeMessage message)
	{
		if (message.Entity.Has<Rigidbody>()) {
			message.Entity.Get<Rigidbody>().updateFlags |= Rigidbody.UpdateFlags.Mass;
		}
	}

	[MessageSubsystem]
	partial void ReadCollisionShapeAdditionMessages(in RemoveCollisionShapeMessage message)
	{
		if (message.Entity.Has<Rigidbody>()) {
			message.Entity.Get<Rigidbody>().updateFlags |= Rigidbody.UpdateFlags.Mass;
		}
	}

	// Update rigidbodies

	[EntitySubsystem]
	partial void UpdateRigidbodies(World world, Entity entity, ref Rigidbody rb, ref Transform transform, [FromWorld] ref WorldPhysics physics)
	{
		var collisionShapeData = entity.Has<CollisionShapesInfo>() ? entity.Get<CollisionShapesInfo>().CollisionShapes : null;

		if (rb.bulletRigidbody == null) {
			var collisionShape = new EmptyShape();

			using var rbInfo = new RigidBodyConstructionInfo(0f, null, collisionShape, Vector3.Zero) {
				LinearSleepingThreshold = 0.1f,
				AngularSleepingThreshold = 1f,
				Friction = rb.Friction,
				RollingFriction = 0f,
				Restitution = 0.1f,
			};

			rb.bulletRigidbody = new RigidBody(rbInfo) {
				UserObject = entity,
				CollisionFlags = CollisionFlags.None,
				LinearVelocity = rb.pendingVelocity ?? default,
				AngularVelocity = rb.pendingAngularVelocity ?? default,
				AngularFactor = rb.pendingAngularFactor ?? Vector3.One,
			};

			rb.pendingVelocity = null;
			rb.pendingAngularVelocity = null;
			rb.pendingAngularFactor = null;

			UpdateCollisionFlags(ref rb);
			UpdateShapes(world, entity, ref rb, collisionShapeData);

			physics.PhysicsWorld.AddRigidBody(rb.bulletRigidbody);
		}

		rb.bulletRigidbody.WorldTransform = transform.WorldMatrix;

		if (rb.updateFlags == 0) {
			return;
		}

		if ((rb.updateFlags & Rigidbody.UpdateFlags.CollisionFlags) != 0) {
			UpdateCollisionFlags(ref rb);
		}

		if ((rb.updateFlags & Rigidbody.UpdateFlags.Mass) != 0) {
			UpdateMass(ref rb);
		}

		if ((rb.updateFlags & Rigidbody.UpdateFlags.CollisionShapes) != 0) {
			physics.PhysicsWorld.RemoveRigidBody(rb.bulletRigidbody);

			UpdateShapes(world, entity, ref rb, collisionShapeData);

			physics.PhysicsWorld.AddRigidBody(rb.bulletRigidbody);
		}
	}

	// Force-activate rigidbodies on demand
	
	[MessageSubsystem]
	partial void ActivateRigidbodies(in ActivateRigidbodyMessage message)
	{
		if (message.Entity.Has<Rigidbody>()) {
			message.Entity.Get<Rigidbody>().bulletRigidbody.Activate();
		}
	}

	private void UpdateShapes(World world, in Entity entity, ref Rigidbody rb, ReadOnlySpan<CollisionShape> collisionShapes)
	{
		var previousShape = rb.bulletRigidbody.CollisionShape;

		CollisionShape resultShape;

		if (collisionShapes.Length == 0) {
			// EmptyShape
			resultShape = new EmptyShape();
		} else {
			// CompoundShape
			var compoundShape = new CompoundShape();

			for (int i = 0; i < collisionShapes.Length; i++) {
				compoundShape.AddChildShape(Matrix4x4.Identity, collisionShapes[i]);
			}

			resultShape = compoundShape;
		}

		rb.bulletRigidbody.CollisionShape = resultShape;

		if (rb.ownsCollisionShape && previousShape != null) {
			previousShape.Dispose();
		}

		rb.updateFlags &= ~Rigidbody.UpdateFlags.CollisionShapes;
		rb.ownsCollisionShape = true;

		world.SendMessage(new ActivateRigidbodyMessage(entity));
	}

	private static void UpdateCollisionFlags(ref Rigidbody rb)
	{
		var flags = rb.bulletRigidbody.CollisionFlags;

		void SetFlag(CollisionFlags flag, bool value)
		{
			if (value) {
				flags |= flag;
			} else {
				flags &= ~flag;
			}
		}

		SetFlag(CollisionFlags.StaticObject, rb.Type == RigidbodyType.Static);
		SetFlag(CollisionFlags.KinematicObject, rb.Type == RigidbodyType.Kinematic);

		rb.bulletRigidbody.CollisionFlags = flags;
		rb.updateFlags &= ~Rigidbody.UpdateFlags.CollisionFlags;
	}

	private static void UpdateMass(ref Rigidbody rb)
	{
		float realMass = rb.Type == RigidbodyType.Dynamic ? rb.Mass : 0f;
		var localInertia = realMass > 0f ? (Vector3)rb.bulletRigidbody.CollisionShape.CalculateLocalInertia(rb.Mass) : Vector3.Zero;

		rb.bulletRigidbody.SetMassProps(realMass, localInertia);

		rb.updateFlags &= ~Rigidbody.UpdateFlags.Mass;
	}
}
