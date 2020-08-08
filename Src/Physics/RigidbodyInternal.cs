using System;
using System.Linq;
using System.Collections.Generic;
using BulletSharp;
using BulletSharp.Math;
using Dissonance.Engine.Core;
using Dissonance.Engine.Structures;
using Vector3 = Dissonance.Engine.Structures.Vector3;

namespace Dissonance.Engine.Physics
{
	internal class RigidbodyInternal : IDisposable
	{
		private class MotionStateInternal : MotionState
		{
			private readonly Transform Transform;

			public MotionStateInternal(Transform transform)
			{
				Transform = transform;
			}

			public override void GetWorldTransform(out Matrix matrix) => matrix = Transform.Parent==null ? Transform.Matrix : Transform.ToWorldSpace(Transform.Matrix);
			public override void SetWorldTransform(ref Matrix matrix) => Transform.Matrix = Transform.Parent==null ? matrix : (Matrix)Transform.ToLocalSpace(matrix);
		}

		//private readonly MotionStateInternal MotionState;
		internal PhysicsEngine engine;
		internal RigidBody btRigidbody;
		internal CollisionShape collisionShape;
		internal List<Collision> collisions;
		internal List<Collision2D> collisions2D;
		internal GameObject gameObject;
		internal RigidbodyBase rigidbody;
		internal bool updateRotation = true;
		internal bool enabled = true;

		private float mass = 1f;
		private RigidbodyType type = RigidbodyType.Static;
		private bool isKinematic;

		public bool Active => btRigidbody.IsActive;
		public float MassFiltered => type==RigidbodyType.Dynamic ? mass : 0f;

		public bool UseGravity {
			get => btRigidbody.Gravity!=BulletSharp.Math.Vector3.Zero;
			set {
				btRigidbody.Gravity = value ? PhysicsEngine.Gravity : Vector3.Zero;

				btRigidbody.ApplyGravity();
			}
		}
		public bool IsKinematic {
			get => isKinematic;
			set {
				if(isKinematic==value) {
					return;
				}

				isKinematic = value;

				if(isKinematic) {
					btRigidbody.CollisionFlags |= CollisionFlags.KinematicObject;
					btRigidbody.ActivationState = ActivationState.DisableDeactivation;
				} else {
					btRigidbody.CollisionFlags &= ~CollisionFlags.KinematicObject;
					btRigidbody.ActivationState = ActivationState.ActiveTag;
				}
			}
		}
		public float Mass {
			get => mass;
			set {
				mass = value;

				UpdateMass();
			}
		}
		public float Friction {
			get => btRigidbody.Friction;
			set => btRigidbody.Friction = value;
		}
		public float Drag {
			get => btRigidbody.LinearDamping;
			set => btRigidbody.SetDamping(value,AngularDrag);
		}
		public float AngularDrag {
			get => btRigidbody.AngularDamping;
			set => btRigidbody.SetDamping(Drag,value);
		}
		public Vector3 Velocity {
			get => btRigidbody.LinearVelocity;
			set {
				if(value!=Vector3.Zero && !btRigidbody.IsActive) {
					btRigidbody.Activate();
				}

				btRigidbody.LinearVelocity = value;
			}
		}
		public Vector3 AngularVelocity {
			get => btRigidbody.AngularVelocity;
			set {
				if(value!=Vector3.Zero && !btRigidbody.IsActive) {
					btRigidbody.Activate();
				}

				btRigidbody.AngularVelocity = value;
			}
		}
		public Vector3 AngularFactor {
			get => btRigidbody.AngularFactor;
			set => btRigidbody.AngularFactor = value;
		}
		public RigidbodyType Type {
			get => type;
			set {
				if(type==value) {
					return;
				}

				type = value;

				if(type==RigidbodyType.Dynamic) {
					btRigidbody.CollisionFlags = CollisionFlags.None;
					btRigidbody.ActivationState = ActivationState.ActiveTag;
				} else if(type==RigidbodyType.Kinematic) {
					btRigidbody.CollisionFlags = CollisionFlags.KinematicObject;
					btRigidbody.ActivationState = ActivationState.DisableDeactivation;
				} else {
					btRigidbody.CollisionFlags = CollisionFlags.StaticObject;
					btRigidbody.ActivationState = ActivationState.DisableDeactivation;
				}

				UpdateMass();
			}
		}

		public RigidbodyInternal(GameObject gameObject)
		{
			this.gameObject = gameObject;

			engine = PhysicsEngine.Instance;

			collisions = new List<Collision>();
			collisions2D = new List<Collision2D>();
			collisionShape = new EmptyShape();

			var motionState = new MotionStateInternal(gameObject.Transform);
			var rbInfo = new RigidBodyConstructionInfo(0f,motionState,collisionShape,Vector3.Zero) {
				LinearSleepingThreshold = 0.1f,
				AngularSleepingThreshold = 1f,
				Friction = 0.6f,
				RollingFriction = 0f,
				Restitution = 0.1f,
			};

			btRigidbody = new RigidBody(rbInfo);
			btRigidbody.CollisionFlags |= CollisionFlags.CustomMaterialCallback;
			btRigidbody.UserObject = this;

			engine.world.AddRigidBody(btRigidbody);
			engine.rigidbodies.Add(this);
		}

		public void ApplyForce(Vector3 force,Vector3 relativePos)
		{
			if(!btRigidbody.IsActive) {
				btRigidbody.Activate();
			}

			btRigidbody.ApplyForce(force,relativePos);
		}
		public void Dispose()
		{
			if(btRigidbody!=null) {
				btRigidbody.Dispose();

				btRigidbody = null;
			}

			if(engine!=null) {
				engine.rigidbodies.Remove(this);

				engine = null;
			}

			collisionShape = null;
		}

		internal void UpdateShape()
		{
			engine.world.RemoveRigidBody(btRigidbody);

			if(collisionShape!=null && (collisionShape is CompoundShape || collisionShape is EmptyShape)) {
				collisionShape.Dispose();
			}

			var colliders = gameObject.GetComponents<Collider>().ToArray();

			if(colliders.Length==0) {
				//EmptyShape

				collisionShape = new EmptyShape();
				//}
				//else if(colliders.Length==1) {
				//	//Use the shape we have
				//
				//	collisionShape = colliders[0].collShape;
				//	//summOffset += collider.offset;
			} else {
				//CompoundShape

				var compoundShape = new CompoundShape();

				for(int i = 0;i<colliders.Length;i++) {
					var collider = colliders[i];
					var collShape = collider.collShape;

					if(collShape!=null) {
						compoundShape.AddChildShape(Matrix4x4.CreateTranslation(collider.Offset),collShape);
					}
				}

				collisionShape = compoundShape;
			}

			float tempMass = MassFiltered;

			var localInertia = MassFiltered<=0f ? BulletSharp.Math.Vector3.Zero : collisionShape.CalculateLocalInertia(tempMass);

			btRigidbody.CollisionShape = collisionShape;
			btRigidbody.SetMassProps(tempMass,localInertia);

			engine.world.AddRigidBody(btRigidbody);
		}
		internal void UpdateMass()
		{
			engine.world.RemoveRigidBody(btRigidbody);

			BulletSharp.Math.Vector3 localInertia = BulletSharp.Math.Vector3.Zero;

			float tempMass = MassFiltered;

			if(tempMass!=0f && collisionShape!=null) {
				localInertia = collisionShape.CalculateLocalInertia(tempMass);
			}

			btRigidbody.SetMassProps(tempMass,localInertia);

			engine.world.AddRigidBody(btRigidbody);
		}
		internal void AddCollision(RigidbodyInternal bodyOther)
		{

		}
	}
}
