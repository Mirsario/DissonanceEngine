using Dissonance.Engine.IO;

namespace Dissonance.Engine
{
	[Receives<SpawnEntityMessage>]
	public sealed class EntitySpawningSystem : GameSystem
	{
		protected internal override void FixedUpdate() => Update();
		protected internal override void RenderUpdate() => Update();

		private void Update()
		{
			foreach (var message in ReadMessages<SpawnEntityMessage>()) {
				var prefab = Assets.Get<PackedEntity>(message.PrefabAssetPath, AssetRequestMode.ImmediateLoad).Value;
				var entity = prefab.Unpack(World);

				message.Action?.Invoke(entity);
			}
		}
	}
}
