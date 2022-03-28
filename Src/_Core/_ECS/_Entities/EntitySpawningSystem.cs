namespace Dissonance.Engine
{
	[Callback<EndRenderUpdateCallback>]
	[Callback<EndFixedUpdateCallback>]
	public sealed class EntitySpawningSystem : GameSystem
	{
		protected override void Execute()
		{
			foreach (var message in ReadMessages<SpawnEntityMessage>()) {
				var entity = message.SourceEntity.Clone(World);

				message.Action?.Invoke(entity);
			}
		}
	}
}
