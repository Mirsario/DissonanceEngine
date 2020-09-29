using Dissonance.Engine.Structures;
using Dissonance.Framework.Windowing.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dissonance.Engine.Input
{
	internal class InputVariables
	{
		//Mouse
		public int mouseWheel;
		public Vector2 mousePosition;
		public bool[] mouseButtons = new bool[InputEngine.MaxMouseButtons];

		//Keyboard
		public Dictionary<Keys, byte> pressedKeys = new Dictionary<Keys, byte>(); //Value is amount of ticks left until released.
		public string inputString = string.Empty;

		//Gamepads
		//public GamePadState[] gamepadStates = new GamePadState[Input.MaxGamepads];

		//Universal
		public HashSet<string> pressedButtons = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		public void Update()
		{
			var pairs = pressedKeys.ToArray();

			foreach(var pair in pairs) {
				byte release = pair.Value;

				if(release > 0) {
					release--;

					if(release == 0) {
						pressedKeys.Remove(pair.Key);
					} else {
						pressedKeys[pair.Key] = release;
					}
				}
			}
		}
		public void CopyTo(InputVariables other, bool reset = true)
		{
			if(reset) {
				other.Reset(false);
			}

			other.mouseWheel = mouseWheel;
			other.mousePosition = mousePosition;
			other.inputString = inputString;

			foreach(var pair in pressedKeys) {
				other.pressedKeys.Add(pair.Key, pair.Value);
			}

			foreach(string str in pressedButtons) {
				other.pressedButtons.Add(str);
			}

			for(int i = 0; i < InputEngine.MaxMouseButtons; i++) {
				other.mouseButtons[i] = mouseButtons[i];
			}
		}
		public void Reset(bool resetArrays = true)
		{
			mouseWheel = 0;
			mousePosition = Vector2.Zero;
			inputString = string.Empty;

			pressedKeys.Clear();
			pressedButtons.Clear();

			if(resetArrays) {
				for(int i = 0; i < InputEngine.MaxMouseButtons; i++) {
					mouseButtons[i] = false;
				}
			}
		}
	}
}