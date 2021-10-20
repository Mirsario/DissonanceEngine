using System;

namespace Dissonance.Engine
{
	public readonly struct SpawnEntityMessage
	{
		public readonly string PrefabAssetPath;
		public readonly Action<Entity> Action;

		public SpawnEntityMessage(string prefabAssetPath, Action<Entity> setupAction = null)
		{
			PrefabAssetPath = prefabAssetPath;
			Action = setupAction;
		}
	}
}
