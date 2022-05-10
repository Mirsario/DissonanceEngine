namespace Dissonance.Engine;

[Callback<EndRenderUpdateCallback>]
[Callback<EndFixedUpdateCallback>]
public sealed partial class EntitySpawningSystem : GameSystem
{
	[MessageSubsystem]
	partial void ReadMessages(in SpawnEntityMessage message, World world)
	{
		var entity = message.SourceEntity.Clone(world);

		if (message.WritePrefab && message.SourceEntity.WorldId == WorldManager.PrefabWorldId) {
			entity.Set(new EntityPrefab(message.SourceEntity.Id));
		}
		
		message.Action?.Invoke(entity);
	}
}
