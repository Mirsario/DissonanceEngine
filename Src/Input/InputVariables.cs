using Dissonance.Framework.Windowing.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dissonance.Engine.Input
{
	internal class InputVariables
	{
		public HashSet<string> PressedButtons { get; } = new(StringComparer.InvariantCultureIgnoreCase);
		public Dictionary<Keys, byte> PressedKeys { get; } = new(); // Value is amount of ticks left until released.

		public int MouseWheel { get; set; }
		public Vector2 MousePosition { get; set; }
		public string InputString { get; set; } = string.Empty;
		public bool[] MouseButtons { get; private set; } = new bool[InputEngine.MaxMouseButtons];

		// Gamepads
		// public GamePadState[] gamepadStates = new GamePadState[Input.MaxGamepads];

		public void Update()
		{
			var pairs = PressedKeys.ToArray();

			foreach (var pair in pairs) {
				byte release = pair.Value;

				if (release > 0) {
					release--;

					if (release == 0) {
						PressedKeys.Remove(pair.Key);
					} else {
						PressedKeys[pair.Key] = release;
					}
				}
			}
		}

		public void CopyTo(InputVariables other, bool reset = true)
		{
			if (reset) {
				other.Reset(false);
			}

			other.MouseWheel = MouseWheel;
			other.MousePosition = MousePosition;
			other.InputString = InputString;

			foreach (var pair in PressedKeys) {
				other.PressedKeys.Add(pair.Key, pair.Value);
			}

			foreach (string str in PressedButtons) {
				other.PressedButtons.Add(str);
			}

			for (int i = 0; i < InputEngine.MaxMouseButtons; i++) {
				other.MouseButtons[i] = MouseButtons[i];
			}
		}

		public void Reset(bool resetArrays = true)
		{
			MouseWheel = 0;
			MousePosition = Vector2.Zero;
			InputString = string.Empty;

			PressedKeys.Clear();
			PressedButtons.Clear();

			if (resetArrays) {
				for (int i = 0; i < InputEngine.MaxMouseButtons; i++) {
					MouseButtons[i] = false;
				}
			}
		}
	}
}
