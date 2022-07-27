using System;
using Dissonance.Engine.IO;

namespace Dissonance.Engine;

public readonly struct SpawnEntityMessage
{
	public static bool DefaultWritePrefab { get; set; } = true;

	public readonly Entity SourceEntity;
	public readonly Action<Entity> Action;
	public readonly bool WritePrefab;

	public SpawnEntityMessage(string prefabAssetPath, Action<Entity> setupAction = null, bool? writePrefab = null)
		: this(Assets.Find<EntityPrefab>(prefabAssetPath).GetValueImmediately(), setupAction, writePrefab) { }

	public SpawnEntityMessage(EntityPrefab prefab, Action<Entity> setupAction = null, bool? writePrefab = null)
		: this((Entity)prefab, setupAction, writePrefab) { }

	public SpawnEntityMessage(Entity sourceEntity, Action<Entity> setupAction = null, bool? writePrefab = null)
	{
		SourceEntity = sourceEntity;
		Action = setupAction;
		WritePrefab = writePrefab ?? DefaultWritePrefab;
	}
}
