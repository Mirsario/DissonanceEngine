#nullable enable

using System;
using System.Reflection;

namespace Dissonance.Engine;

[ModuleDependency<WorldManager>]
[ModuleDependency<Callbacks>]
public sealed class Systems : EngineModule
{
	public static uint SystemCount => SystemStorage.SystemCount;

	protected override void Init()
	{
		SystemStorage.Initialize();
	}

	protected override void InitializeForAssembly(Assembly assembly)
	{
		SystemStorage.InitializeForAssembly(assembly);
	}

	// Access

	public static SystemHandle Get(MethodInfo method)
		=> SystemStorage.GetSystem(method);

	public static bool TryGet(MethodInfo method, out SystemHandle result)
		=> SystemStorage.TryGetSystem(method, out result);

	// Modification

	public static void AddTagsToSystem(SystemHandle system, ReadOnlySpan<Tag> tags)
	{
		var list = system.Description.tags;

		list.EnsureCapacity(list.Count + tags.Length);

		for (int i = 0; i < tags.Length; i++) {
			var tag = tags[i];

			list.Add(tag);
			Tags.AddSystem(tag, system);
		}
	}

	public static void AddTagRequirementsToSystem(SystemHandle system, ReadOnlySpan<Tag> tags)
	{
		var list = system.Description.requiredTags;

		list.EnsureCapacity(list.Count + tags.Length);

		for (int i = 0; i < tags.Length; i++) {
			list.Add(tags[i]);
		}
	}
}
