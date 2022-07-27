using System;
using System.Collections.Generic;
using Silk.NET.GLFW;

namespace Dissonance.Engine.Input;

partial class InputEngine
{
	private static Dictionary<string, int> signalIdByName = new(StringComparer.InvariantCultureIgnoreCase);
	private static List<(Func<object, float> getter, object arg)> signals = new();

	private static void InitSignals()
	{
		// Register key signals
		foreach (Keys key in Enum.GetValues(typeof(Keys))) {
			RegisterSignal(key.ToString(), arg => GetKey((Keys)arg) ? 1f : 0f, key);
		}

		// Register mouse signals
		foreach (MouseButton button in Enum.GetValues(typeof(MouseButton))) {
			RegisterSignal($"Mouse{button}", arg => GetMouseButton((MouseButton)arg) ? 1f : 0f, button);
		}

		RegisterSignal("MouseX", arg => MouseDelta.X);
		RegisterSignal("MouseY", arg => MouseDelta.Y);
		RegisterSignal("MouseWheel", arg => MouseWheel);
	}

	public static void RegisterSignal(string nameId, Func<object, float> valueGetter, object arg = null)
	{
		var tuple = (valueGetter, arg);

		if (signalIdByName.TryGetValue(nameId, out int id)) {
			signals[id] = tuple;

			return;
		}

		signalIdByName[nameId] = signals.Count;

		signals.Add(tuple);
	}

	public static float GetSignal(string nameId)
	{
		if (!signalIdByName.TryGetValue(nameId, out int id)) {
			return 0f;
		}

		var (getter, arg) = signals[id];

		return getter(arg);
	}
}
