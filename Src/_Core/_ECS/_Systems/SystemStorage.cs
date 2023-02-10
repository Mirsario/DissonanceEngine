#nullable enable

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Dissonance.Engine;

internal static class SystemStorage
{
	private static readonly Dictionary<Type, SystemHandle> systemsByType = new();
	private static readonly Dictionary<MethodInfo, SystemHandle> systemsByMethod = new();

	private static SystemDescription[] systemDescriptions = new SystemDescription[64];

	public static uint SystemCount { get; private set; }

	public static void Initialize()
	{
		for (uint i = 1, loopEnd = SystemCount; i <= loopEnd; i++) {
			ref var description = ref systemDescriptions[i];

			foreach (var attribute in description.Method.GetCustomAttributes()) {
				if (attribute is ISystemAttribute systemAttribute) {
					systemAttribute.ConfigureSystem(Unsafe.As<uint, SystemHandle>(ref i));
				}
			}
		}
	}

	public static void InitializeForAssembly(Assembly assembly)
	{
		const BindingFlags AnyBindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		var delegateParameters = typeof(SystemDelegate).GetMethod("Invoke")!.GetParameters();

		foreach (var type in assembly.GetTypes()) {
			foreach (var method in type.GetMethods(AnyBindingFlags)) {
				var systemAttribute = method.GetCustomAttribute<SystemAttribute>();

				if (systemAttribute == null) {
					continue;
				}

				if (method.GetCustomAttribute<IgnoredSystemAttribute>() != null) {
					continue;
				}

				if (!method.IsStatic) {
					throw new InvalidOperationException($"{nameof(SystemAttribute)}-marked method '{method.Name}' must be static.");
				}

				var methodParameters = method.GetParameters();

				static bool CompareParameters(ReadOnlySpan<ParameterInfo> spanA, ReadOnlySpan<ParameterInfo> spanB)
				{
					if (spanA.Length != spanB.Length) {
						return false;
					}

					for (int i = 0; i < spanA.Length; i++) {
						var a = spanA[i];
						var b = spanB[i];

						if (a.ParameterType != b.ParameterType || a.IsOut != b.IsOut || a.IsIn != b.IsIn) {
							return false;
						}
					}

					return true;
				}

				if (method.ReturnType != typeof(SystemResult) || !CompareParameters(methodParameters, delegateParameters)) {
					throw new InvalidOperationException($"{nameof(SystemAttribute)}-marked method '{method.Name}' must match the '{nameof(SystemDelegate)}' delegate.");
				}

				if (!AutoloadAttribute.MemberNeedsAutoloading(method, true)) {
					continue;
				}

				uint id = ++SystemCount;
				var handle = new SystemHandle(id);

				if (systemDescriptions.Length <= id) {
					Array.Resize(ref systemDescriptions, (int)BitOperations.RoundUpToPowerOf2(id + 1));
				}
			
				var functionDelegate = method.CreateDelegate<SystemDelegate>();
				var description = new SystemDescription(method, functionDelegate);

				systemDescriptions[id] = description;
				systemsByMethod[method] = handle;
			}
		}
	}

	public static SystemHandle GetSystem(MethodInfo method)
		=> TryGetSystem(method, out var result) ? result : throw new InvalidOperationException($"System '{method.Name}.{method.DeclaringType!.Name}' was not found.");

	public static bool TryGetSystem(MethodInfo method, out SystemHandle result)
		=> systemsByMethod.TryGetValue(method, out result);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly SystemDescription GetDescription(SystemHandle handle)
		=> ref  systemDescriptions[handle.Id];
}
