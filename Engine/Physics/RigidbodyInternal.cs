using System;
using System.Collections.Generic;
using BulletSharp;

namespace GameEngine
{
	//TO CONSIDER: Make this be derived from Bullet's RigidBody ?
	internal class RigidbodyInternal : IDisposable
	{
		private class MotionStateInternal : MotionState
		{
			public readonly Transform transform;
			
			public override Matrix WorldTransform {
				get => transform.parent==null ? transform._matrix : transform.ToWorldSpace(transform._matrix);
				set => transform._matrix = transform.parent==null ? value : (Matrix)transform.ToLocalSpace(value);
			}

			public MotionStateInternal(Transform transform)
			{
				this.transform = transform;
			}
		}
		//private readonly MotionStateInternal motionState;
		internal RigidBody btRigidbody;
		internal CollisionShape collisionShape;
		internal List<Collision> collisions;
		internal List<Collision2D> collisions2D;
		internal bool updateRotation = true;
		internal bool enabled = true;
		
		internal GameObject gameObject;
		internal RigidbodyBase rigidbody;

		#region Properties
		public bool Active => btRigidbody.IsActive;

		private float _mass = 1f;
		public float Mass {
			get => _mass;
			set {
				_mass = value;
				UpdateMass();
			}
		}
		public float MassFiltered => _type==RigidbodyType.Dynamic ? _mass : 0f;
		public Vector3 Velocity {
			get => btRigidbody.LinearVelocity;
			set {
				if(value!=Vector3.zero && !btRigidbody.IsActive) {
					btRigidbody.Activate();
				}
				btRigidbody.LinearVelocity = value;
			}
		}
		public Vector3 AngularVelocity {
			get => btRigidbody.AngularVelocity;
			set {
				if(value!=Vector3.zero && !btRigidbody.IsActive) {
					btRigidbody.Activate();
				}
				btRigidbody.AngularVelocity = value;
			}
		}
		public Vector3 AngularFactor {
			get => btRigidbody.AngularFactor;
			set => btRigidbody.AngularFactor = value;
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
		public bool UseGravity {
			get => btRigidbody.Gravity!=BulletSharp.Vector3.Zero;
			set {
				if(value) {
					btRigidbody.Gravity = Physics.Gravity;
				}else{
					btRigidbody.Gravity = Vector3.zero;
				}
				btRigidbody.ApplyGravity();
			}
		}
		private bool _isKinematic;
		public bool IsKinematic {
			get => _isKinematic;
			set {
				if(_isKinematic!=value) {
					_isKinematic = value;
					if(_isKinematic) {
						btRigidbody.CollisionFlags = btRigidbody.CollisionFlags | CollisionFlags.KinematicObject;
						btRigidbody.ActivationState = ActivationState.DisableDeactivation;
					}else{
						btRigidbody.CollisionFlags = btRigidbody.CollisionFlags & ~CollisionFlags.KinematicObject;
						btRigidbody.ActivationState = ActivationState.ActiveTag;
					}
				}
			}
		}
		private RigidbodyType _type = RigidbodyType.Static;
		public RigidbodyType Type {
			get => _type;
			set {
				if(_type!=value) {
					_type = value;
					if(_type==RigidbodyType.Dynamic) {
						btRigidbody.CollisionFlags = CollisionFlags.None;
						btRigidbody.ActivationState = ActivationState.ActiveTag;
					}else if(_type==RigidbodyType.Kinematic) {
						btRigidbody.CollisionFlags = CollisionFlags.KinematicObject;
						btRigidbody.ActivationState = ActivationState.DisableDeactivation;
					}else{
						btRigidbody.CollisionFlags = CollisionFlags.StaticObject;
						btRigidbody.ActivationState = ActivationState.DisableDeactivation;
					}
					//Debug.Log("Setting type to "+_type+" for "+gameObject.name);
					UpdateMass();
				}
			}
		}
		#endregion

		public RigidbodyInternal(GameObject gameObject)
		{
			this.gameObject = gameObject;
			collisions = new List<Collision>();
			collisions2D = new List<Collision2D>();

			//At first we just create a rigidbody with an empty shape and 1f mass (zero local inertia)
			collisionShape = new EmptyShape();
			var motionState = new MotionStateInternal(gameObject.transform);
			var rbInfo = new RigidBodyConstructionInfo(0f,motionState,collisionShape,Vector3.zero) {
				LinearSleepingThreshold = 0.1f,
				AngularSleepingThreshold = 1f,
				Friction = 0.6f,
				RollingFriction = 0f,
				Restitution = 0.1f,
			};
			btRigidbody = new RigidBody(rbInfo);
			btRigidbody.CollisionFlags |= CollisionFlags.CustomMaterialCallback;
			btRigidbody.UserObject = this;

			Physics.world.AddRigidBody(btRigidbody);
			Physics.rigidbodies.Add(this);
		}
		internal void UpdateShape()
		{
			Physics.world.RemoveRigidBody(btRigidbody);

			if(collisionShape!=null && (collisionShape is CompoundShape || collisionShape is EmptyShape)) {
				collisionShape.Dispose();
			}
			var summOffset = Vector3.zero;
			var colliders = gameObject.GetComponents<Collider>();

			if(colliders.Length==0) {
				//EmptyShape
				collisionShape = new EmptyShape();
				//Debug.Log("Using empty shape");
			/*}else if(colliders.Length==1) {
				//Use the shape we have
				var collider = colliders[0];
				collisionShape = collider.collShape;
				summOffset += (OpenTK.Vector3)collider.offset;*/
			}else{
				//CompoundShape
				//Debug.Log("Creating compound shape");
				var compoundShape = new CompoundShape();
				for(int i=0;i<colliders.Length;i++) {
					var collShape = colliders[i].collShape;
					if(collShape!=null) {
						compoundShape.AddChildShape(Matrix4x4.CreateTranslation(colliders[i].offset),collShape);
						summOffset += colliders[i].offset;
					}
				}
				collisionShape = compoundShape;
			}
			float tempMass = MassFiltered;
			//Debug.Log(gameObject.name+": offset is "+summOffset);
			var localInertia = MassFiltered<=0f ? BulletSharp.Vector3.Zero : collisionShape.CalculateLocalInertia(tempMass);
			btRigidbody.CollisionShape = collisionShape;
			btRigidbody.SetMassProps(tempMass,localInertia);

			Physics.world.AddRigidBody(btRigidbody);
		}
		internal void UpdateMass()
		{
			Physics.world.RemoveRigidBody(btRigidbody);
				
			BulletSharp.Vector3 localInertia = Vector3.zero;
			float tempMass = MassFiltered;
			if(tempMass!=0f && collisionShape!=null) {
				localInertia = collisionShape.CalculateLocalInertia(tempMass);
			}
			//Debug.Log("setting to-"+tempMass);
			btRigidbody.SetMassProps(tempMass,localInertia);
			
			Physics.world.AddRigidBody(btRigidbody);

			//Debug.Log("mass check-"+rigidbody.InvMass);
		}
		/*public void Enable()
		{
			Physics.rigidbodies.Add(this);
		}
		public void Disable()
		{
			Physics.rigidbodies.Remove(this);
		}*/
		public void ApplyForce(Vector3 force,Vector3 relativePos)
		{
			if(!btRigidbody.IsActive) {
				btRigidbody.Activate();
			}
			btRigidbody.ApplyForce(force,relativePos);
		}
		public void Dispose()
		{
			Physics.rigidbodies.Remove(this);
		}
		internal void AddCollision(RigidbodyInternal bodyOther)
		{
			
		}
	}
}

