using System;

namespace Dissonance.Engine
{
	public readonly struct SpawnEntityMessage
	{
		public readonly string PrefabName;
		public readonly Action<Entity> Action;

		public SpawnEntityMessage(string prefabAssetPath, Action<Entity> setupAction = null)
		{
			PrefabName = prefabAssetPath;
			Action = setupAction;
		}
	}
}
