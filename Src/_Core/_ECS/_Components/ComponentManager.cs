using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine;

public unsafe sealed partial class ComponentManager : EngineModule
{
	private static partial class ComponentData<T> where T : struct
	{
		public static int TypeId;

		static ComponentData()
		{
			TypeId = ComponentTypeDataById.Count;

			var typeData = new ComponentTypeData(
				typeof(T),
				GetComponentGlobalFunctions(),
				GetComponentWorldFunctions(),
				GetComponentEntityFunctions()
			);

			ComponentTypeDataById.Add(typeData);
		}
	}

	private readonly record struct ComponentTypeData(Type Type, ComponentGlobalFunctions GlobalFunctions, ComponentWorldFunctions WorldFunctions, ComponentEntityFunctions EntityFunctions)
	{
		public override string ToString() => Type.ToString();
	}

	private static readonly List<ComponentTypeData> ComponentTypeDataById = new();
	private static readonly Dictionary<string, Type> StructureTypesByName = new();

	public static int ComponentTypeCount => ComponentTypeDataById.Count;

	protected override void InitializeForAssembly(Assembly assembly)
	{
		// By-name type lookups are used in prefab parsing.
		foreach (var type in assembly.GetTypes()) {
			if (!type.IsValueType || type.IsAbstract || type.IsByRefLike || type.IsGenericTypeDefinition) {
				continue;
			}

			string name = type.Name;

			if (type.IsNested) {
				Type declaringType = type.DeclaringType;

				while (declaringType != null) {
					name = $"{declaringType.Name}.{name}";

					declaringType = declaringType.DeclaringType;
				}
			}

			if (!StructureTypesByName.TryGetValue(type.Name, out var existingType)) {
				StructureTypesByName[name] = type;
			} else {
				//TODO: Use minimal unique paths.
				StructureTypesByName[name] = null;
				StructureTypesByName[type.FullName] = type;
				StructureTypesByName[existingType.FullName] = existingType;
			}
		}
	}

	public static Type GetComponentTypeFromName(string name)
	{
		if (StructureTypesByName.TryGetValue(name, out var type)) {
			if (type == null) {
				throw new ArgumentException($"Component name '{name}' is ambiguous.");
			}

			return type;
		}

		throw new KeyNotFoundException($"Couldn't find component with the provided name '{name}'.");
	}
	
	public static bool TryGetComponentTypeFromName(string name, out Type type)
		=> StructureTypesByName.TryGetValue(name, out type) && type != null;

	public static int GetComponentId<T>() where T : struct
		=> ComponentData<T>.TypeId;
}
