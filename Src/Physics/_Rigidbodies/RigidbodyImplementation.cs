using System;
using BulletSharp;

namespace Dissonance.Engine.Physics;

internal static partial class RigidbodyImplementation
{
	// Set UpdateShapes to true whenever collision shapes have been modified.

	[MessageSystem, CalledIn<PhysicsUpdate>, Tags("RigidbodyUpdateFlags"), RequiresTags("ColliderUpdate")]
	static partial void ReadCollisionShapeRemovalMessages(in RemoveCollisionShapeMessage message)
	{
		if (message.Entity.Has<Rigidbody>()) {
			message.Entity.Get<Rigidbody>().updateFlags |= Rigidbody.UpdateFlags.Mass;
		}
	}

	[MessageSystem, CalledIn<PhysicsUpdate>, Tags("RigidbodyUpdateFlags"), RequiresTags("ColliderUpdate")]
	static partial void ReadCollisionShapeAdditionMessages(in RemoveCollisionShapeMessage message)
	{
		if (message.Entity.Has<Rigidbody>()) {
			message.Entity.Get<Rigidbody>().updateFlags |= Rigidbody.UpdateFlags.Mass;
		}
	}

	// Update rigidbodies

	[EntitySystem, CalledIn<PhysicsUpdate>, Tags("RigidbodyUpdate"), RequiresTags("RigidbodyUpdateFlags", "CollisionShapesInfo")]
	static partial void UpdateRigidbodies(World world, Entity entity, ref Rigidbody rb, ref Transform transform, [FromWorld] ref WorldPhysics physics)
	{
		var collisionShapes = entity.Has<CollisionShapesInfo>() ? entity.Get<CollisionShapesInfo>().CollisionShapes : null;

		if (rb.bulletRigidbody == null) {
			var collisionShape = GetOrCreateCollisionShape(collisionShapes, out rb.ownsCollisionShape);

			CalculateMassProperties(in rb, collisionShape, out float mass, out Vector3 localInertia);

			var worldMatrix = transform.WorldMatrix;

			worldMatrix.ClearScale();

			using var rbInfo = new RigidBodyConstructionInfo(mass, null, collisionShape, localInertia) {
				StartWorldTransform = (System.Numerics.Matrix4x4)worldMatrix,
				Friction = rb.Friction,
				LinearSleepingThreshold = 0.1f,
				AngularSleepingThreshold = 1f,
				RollingFriction = 0f,
				Restitution = 0.1f,
			};

			rb.bulletRigidbody = new RigidBody(rbInfo) {
				UserObject = entity,
				CollisionFlags = CollisionFlags.CustomMaterialCallback,
				LinearVelocity = rb.pendingVelocity ?? default,
				AngularVelocity = rb.pendingAngularVelocity ?? default,
				AngularFactor = rb.pendingAngularFactor ?? Vector3.One,
			};

			UpdateCollisionFlags(ref rb);

			rb.pendingVelocity = null;
			rb.pendingAngularVelocity = null;
			rb.pendingAngularFactor = null;

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

			UpdateShapes(ref rb, collisionShapes);

			physics.PhysicsWorld.AddRigidBody(rb.bulletRigidbody);
		}

		rb.bulletRigidbody.Activate();
	}

	// Force-activate rigidbodies on demand

	[MessageSystem, CalledIn<PhysicsUpdate>, RequiresTags("RigidbodyUpdate")]
	static partial void ActivateRigidbodies(in ActivateRigidbodyMessage message)
	{
		if (message.Entity.Has<Rigidbody>()) {
			message.Entity.Get<Rigidbody>().bulletRigidbody.Activate();
		}
	}

	private static CollisionShape GetOrCreateCollisionShape(ReadOnlySpan<CollisionShape> collisionShapes, out bool ownsCollisionShape)
	{
		CollisionShape resultShape;

		if (collisionShapes.Length == 0) {
			// EmptyShape
			resultShape = new EmptyShape();
			ownsCollisionShape = true;
		} else if (collisionShapes.Length == 1) {
			resultShape = collisionShapes[0];
			ownsCollisionShape = false;
		} else {
			// CompoundShape
			var compoundShape = new CompoundShape();

			for (int i = 0; i < collisionShapes.Length; i++) {
				compoundShape.AddChildShape(System.Numerics.Matrix4x4.Identity, collisionShapes[i]);
			}

			resultShape = compoundShape;
			ownsCollisionShape = true;
		}

		return resultShape;
	}

	private static void UpdateShapes(ref Rigidbody rb, ReadOnlySpan<CollisionShape> collisionShapes)
	{
		if (rb.ownsCollisionShape) {
			rb.bulletRigidbody.CollisionShape?.Dispose();
		}

		rb.bulletRigidbody.CollisionShape = GetOrCreateCollisionShape(collisionShapes, out rb.ownsCollisionShape);

		rb.bulletRigidbody.Activate();

		rb.updateFlags &= ~Rigidbody.UpdateFlags.CollisionShapes;
	}

	private static void UpdateCollisionFlags(ref Rigidbody rb)
	{
		var flags = rb.bulletRigidbody.CollisionFlags;
		var flagsBackup = flags;

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

		if (flags != flagsBackup) {
			rb.bulletRigidbody.CollisionFlags = flags;
		}

		rb.updateFlags &= ~Rigidbody.UpdateFlags.CollisionFlags;
	}

	private static void CalculateMassProperties(in Rigidbody rb, CollisionShape collisionShape, out float mass, out Vector3 localInertia)
	{
		mass = rb.Type == RigidbodyType.Dynamic ? rb.Mass : 0f;
		localInertia = mass > 0f ? (Vector3)collisionShape.CalculateLocalInertia(rb.Mass) : Vector3.Zero;
	}

	private static void UpdateMass(ref Rigidbody rb)
	{
		CalculateMassProperties(in rb, rb.bulletRigidbody.CollisionShape, out float mass, out Vector3 localInertia);

		rb.bulletRigidbody.SetMassProps(mass, localInertia);

		rb.updateFlags &= ~Rigidbody.UpdateFlags.Mass;
	}
}
