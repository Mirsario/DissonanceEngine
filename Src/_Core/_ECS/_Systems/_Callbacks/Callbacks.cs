#nullable enable

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Dissonance.Engine;

public sealed class Callbacks : EngineModule
{
	protected override void InitializeForAssembly(Assembly assembly)
	{
		CallbackStorage.InitializeForAssembly(assembly);
	}

	// Access

	public static CallbackHandle Get<T>() where T : Delegate
		=> CallbackStorage.GetCallback<T>();

	public static CallbackHandle Get(Type type)
		=> CallbackStorage.GetCallback(type);

	public static bool TryGet<T>(out CallbackHandle result)
		=> CallbackStorage.TryGetCallback<T>(out result);

	public static bool TryGet(Type type, out CallbackHandle result)
		=> CallbackStorage.TryGetCallback(type, out result);

	// Modification

	public static void AddSystemsToCallback(CallbackHandle handle, ReadOnlySpan<SystemHandle> systems)
	{
		ref var data = ref handle.Data;
		var list = data.Description.systems;

		list.EnsureCapacity(list.Count + systems.Length);

		for (int i = 0; i < systems.Length; i++) {
			list.Add(systems[i]);
		}

		data.NeedsSorting = true;
	}

	// Execution

	public static void Execute<T>() where T : Delegate
		=> Execute(Get<T>());

	public static void Execute(CallbackHandle callback)
	{
		ref var data = ref callback.Data;

		if (data.NeedsSorting) {
			Sort(ref data);

			data.NeedsSorting = false;
		}

		var systems = callback.Description.Systems;

		for (int i = 0; i < systems.Length; i++) {
			systems[i].Description.Function();
		}
	}

	// Sorting

	private unsafe static void Sort(ref CallbackData data)
	{
		var systems = data.Description.systems;
		int subscriberCount = systems.Count;
		int systemCount = (int)Systems.SystemCount;

		// Allocate all needed bytes.

		int dataLength = (subscriberCount * sizeof(SystemHandle)) + (systemCount * sizeof(byte));
		byte* tempData = stackalloc byte[dataLength];

		T* ReserveSpan<T>(int length) where T : unmanaged
		{
			var span = (T*)tempData;

			tempData += length * Marshal.SizeOf<T>();

			return span;
		}

		// Create a copy of the systems list.

		SystemHandle* systemsCopy = ReserveSpan<SystemHandle>(subscriberCount);

		for (int i = 0; i < subscriberCount; i++) {
			systemsCopy[i] = systems[i];
		}

		// Prepare system information.

		byte* systemStates = ReserveSpan<byte>(systemCount); // System ID -> State
		int nextSystemIndex = 0;

		const byte BitVisited = 0b__10000000; // Whether the system has been visited.
		const byte BitSortable = 0b_01000000; // Whether the system is to be sorted.
		const byte BitSortableInverse = unchecked((byte)~BitSortable);

		for (int i = 0; i < subscriberCount; i++) {
			systemStates[systems[i].Id] = BitSortable;
		}

		// Execute sorting.

		void Recursion(SystemHandle system)
		{
			ref byte state = ref systemStates[system.Id];

			if ((state & BitVisited) != 0) {
				return;
			}

			state |= BitVisited;

			ref readonly var description = ref system.Description;

			for (int i = 0; i < description.RequiredTags.Length; i++) {
				var tag = description.RequiredTags[i];
				var tagSystems = Tags.GetTagSystems(tag);

				for (int j = 0; j < tagSystems.Length; j++) {
					Recursion(tagSystems[j]);
				}
			}

			if ((state & BitSortable) != 0) {
				systems[nextSystemIndex++] = system;
				state &= BitSortableInverse;
			}
		}

		for (int i = 0; i < subscriberCount; i++) {
			Recursion(systemsCopy[i]);
		}
	}
}
