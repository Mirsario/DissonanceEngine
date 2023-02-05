#nullable enable

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Dissonance.Engine;

internal static class CallbackStorage
{
	private static readonly Dictionary<Type, CallbackHandle> callbacksByType = new();

	private static CallbackData[] callbackData = new CallbackData[8];

	public static uint CallbackCount { get; private set; }

	public static void InitializeForAssembly(Assembly assembly)
	{
		foreach (var type in assembly.GetTypes()) {
			if (!typeof(Delegate).IsAssignableFrom(type)) {
				continue;
			}

			if (type.GetCustomAttribute<CallbackDeclarationAttribute>() == null) {
				continue;
			}

			uint id = ++CallbackCount;
			var handle = new CallbackHandle(id);

			if (callbackData.Length <= id) {
				Array.Resize(ref callbackData, (int)BitOperations.RoundUpToPowerOf2(id + 1));
			}

			var data = new CallbackData {
				Description = new CallbackDescription(type)
			};

			callbackData[id] = data;
			callbacksByType[type] = handle;
		}
	}

	// Access

	public static CallbackHandle GetCallback<T>() where T : Delegate
		=> GetCallback(typeof(T));

	public static CallbackHandle GetCallback(Type type)
		=> TryGetCallback(type, out var result) ? result : throw new InvalidOperationException($"Callback '{type.Name}' was not found.");

	public static bool TryGetCallback<T>(out CallbackHandle result)
		=> TryGetCallback(typeof(T), out result);

	public static bool TryGetCallback(Type type, out CallbackHandle result)
		=> callbacksByType.TryGetValue(type, out result);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref CallbackData GetData(CallbackHandle handle)
		=> ref callbackData[handle.Id];
}
