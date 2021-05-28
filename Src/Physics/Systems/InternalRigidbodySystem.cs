using BulletSharp;

namespace Dissonance.Engine.Physics.Systems
{
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
						Restitution = 0.1f,
					};

					rb.BulletRigidbody = new RigidBody(rbInfo) {
						UserObject = this,
					};

					rb.BulletRigidbody.CollisionFlags |= CollisionFlags.CustomMaterialCallback;

					physics.PhysicsWorld.AddRigidBody(rb.BulletRigidbody);
				}
			}
		}
	}
}
