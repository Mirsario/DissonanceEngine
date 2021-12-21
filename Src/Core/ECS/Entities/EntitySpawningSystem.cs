using Dissonance.Engine.IO;

namespace Dissonance.Engine
{
	[Callback<EndRenderUpdateCallback>]
	[Callback<EndFixedUpdateCallback>]
	public sealed class EntitySpawningSystem : GameSystem
	{
		protected override void Execute()
		{
			foreach (var message in ReadMessages<SpawnEntityMessage>()) {
				var prefab = Assets.Find<EntityPrefab>(message.PrefabName).GetValueImmediately();
				var entity = prefab.Clone(World);

				message.Action?.Invoke(entity);
			}
		}
	}
}
