namespace Dissonance.Engine.Physics
{
	[Reads(typeof(Rigidbody))]
	[Writes(typeof(Rigidbody), typeof(RigidbodyInternal))]
	[Sends(typeof(CreateInternalRigidbodyMessage), typeof(SetRigidbodyTypeMessage), typeof(SetRigidbodyMassMessage))]
	[Receives(typeof(ComponentRemovedMessage<Rigidbody>))]
	public sealed class RigidbodySystem : GameSystem
	{
		private EntitySet entities;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<Rigidbody>() && e.Has<Transform>());
		}

		protected internal override void FixedUpdate()
		{
			foreach(var entity in entities.ReadEntities()) {
				ref var rigidbody = ref entity.Get<Rigidbody>();

				if(!entity.Has<RigidbodyInternal>()) {
					entity.Set(RigidbodyInternal.Create());
					SendMessage(new CreateInternalRigidbodyMessage(entity));
				}

				if(rigidbody.IsKinematic != rigidbody.wasKinematic) {
					SendMessage(new SetRigidbodyTypeMessage(entity, rigidbody.IsKinematic ? RigidbodyType.Kinematic : RigidbodyType.Dynamic)); 

					rigidbody.wasKinematic = rigidbody.IsKinematic;
				}

				if(rigidbody.Mass != rigidbody.lastMass) {
					SendMessage(new SetRigidbodyMassMessage(entity, rigidbody.Mass));

					rigidbody.lastMass = rigidbody.Mass;
				}
			}

			foreach(var message in ReadMessages<ComponentRemovedMessage<Rigidbody>>()) {
				if(!message.Entity.Has<Rigidbody>()) {
					SendMessage(new SetRigidbodyTypeMessage(message.Entity, RigidbodyType.Static));
				}
			}
		}
	}
}
