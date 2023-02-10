namespace Dissonance.Engine;

static partial class EntitySpawning
{
	[MessageSystem, CalledIn<FixedUpdate>, RequiresTags("SpawnsEntities")]
	static partial void SpawnEntities(in SpawnEntityMessage message, World world)
	{
		var entity = message.SourceEntity.Clone(world);

		if (message.WritePrefab && message.SourceEntity.WorldId == WorldManager.PrefabWorldId) {
			entity.Set(new EntityPrefab(message.SourceEntity.Id));
		}

		message.Action?.Invoke(entity);
	}
}
