using BulletSharp;

namespace Dissonance.Engine.Physics
{
	[Reads(typeof(BoxCollider))]
	[Writes(typeof(BoxCollider))]
	[Sends(typeof(AddCollisionShapeMessage), typeof(RemoveCollisionShapeMessage))]
	public sealed class BoxColliderSystem : GameSystem
	{
		private EntitySet entities;

		public override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<BoxCollider>());
		}

		public override void FixedUpdate()
		{
			foreach(var entity in entities.ReadEntities()) {
				ref var collider = ref entity.Get<BoxCollider>();
				bool noShape = collider.boxShape == null;

				if(noShape || collider.needsUpdate) {
					if(!noShape) {
						SendMessage(new RemoveCollisionShapeMessage(entity, collider.boxShape));
						collider.boxShape.Dispose();
					}

					collider.boxShape = new BoxShape(collider.Size * 0.5f);

					SendMessage(new AddCollisionShapeMessage(entity, collider.boxShape));

					collider.needsUpdate = false;
				}
			}
		}
	}
}
