using System;
using Dissonance.Engine.IO;

namespace Dissonance.Engine
{
	public readonly struct SpawnEntityMessage
	{
		public readonly Entity SourceEntity;
		public readonly Action<Entity> Action;

		public SpawnEntityMessage(string prefabAssetPath, Action<Entity> setupAction = null)
			: this(Assets.Find<EntityPrefab>(prefabAssetPath).GetValueImmediately(), setupAction) { }

		public SpawnEntityMessage(EntityPrefab prefab, Action<Entity> setupAction = null)
			: this((Entity)prefab, setupAction) { }

		public SpawnEntityMessage(Entity sourceEntity, Action<Entity> setupAction = null)
		{
			SourceEntity = sourceEntity;
			Action = setupAction;
		}
	}
}
