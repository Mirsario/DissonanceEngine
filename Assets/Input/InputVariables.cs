using System;
using System.Collections.Generic;

namespace GameEngine
{
	internal class InputVariables
	{
		//Mouse
		//public MouseState cursorState;
		//public MouseState mouseState;
		//public MouseState mouseStatePrev;
		public int mouseWheel;
		public Vector2 mousePosition;
		public Vector2 mouseDelta;
		public bool[] mouseButtons = new bool[Input.MaxMouseButtons];
		public bool[] mouseButtonsPrev = new bool[Input.MaxMouseButtons];

		//Keyboard
		public HashSet<Keys> pressedKeys = new HashSet<Keys>();
		public HashSet<Keys> pressedKeysPrev = new HashSet<Keys>();
		public string inputString = "";

		//Gamepads
		//public GamePadState[] gamepadStates = new GamePadState[Input.MaxGamepads];

		//Universal
		public HashSet<string> pressedButtons = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		public HashSet<string> pressedButtonsPrev = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
	}
}