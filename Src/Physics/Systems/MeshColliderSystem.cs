namespace Dissonance.Engine.Physics
{
	[Reads(typeof(MeshCollider))]
	[Writes(typeof(MeshCollider))]
	[Sends(typeof(AddCollisionShapeMessage), typeof(RemoveCollisionShapeMessage))]
	public sealed class MeshColliderSystem : GameSystem
	{
		private EntitySet entities;

		public override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<MeshCollider>());
		}

		public override void FixedUpdate()
		{
			foreach(var entity in entities.ReadEntities()) {
				ref var collider = ref entity.Get<MeshCollider>();

				if(collider.needsUpdate) {
					if(collider.lastCollisionShape != null) {
						SendMessage(new RemoveCollisionShapeMessage(entity, collider.lastCollisionShape));
					}

					var collisionShape = collider.CollisionMesh?.collisionShape;

					if(collisionShape != null) {
						SendMessage(new AddCollisionShapeMessage(entity, collisionShape));
					}

					collider.needsUpdate = false;
				}
			}
		}
	}
}
