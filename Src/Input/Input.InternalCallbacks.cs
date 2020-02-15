using System;
using System.Text;
using Dissonance.Framework.Windowing;
using Dissonance.Framework.Windowing.Input;

namespace Dissonance.Engine
{
	partial class Input
	{
		internal static void InitCallbacks()
		{
			GLFW.SetCursorPosCallback(Game.window,MousePositionCallback);
			GLFW.SetMouseButtonCallback(Game.window,MouseButtonCallback);
			GLFW.SetKeyCallback(Game.window,KeyCallback);
			GLFW.SetCharCallback(Game.window,KeyStringCallback);
		}

		private static void MousePositionCallback(IntPtr window,double x,double y)
		{
			var value = new Vector2((float)x,(float)y);

			fixedInput.mousePosition = value;
			renderInput.mousePosition = value;
		}
		private static void MouseButtonCallback(IntPtr window,MouseButton button,MouseAction action,KeyModifiers mods)
		{
			bool value = action==MouseAction.Press;

			fixedInput.mouseButtons[(int)button] = value;
			renderInput.mouseButtons[(int)button] = value;
		}
		private static void KeyStringCallback(IntPtr window,uint codePoint)
		{
			byte[] bytes = BitConverter.GetBytes(codePoint);

			string text = Encoding.UTF32.GetString(bytes);

			fixedInput.inputString += text;
			renderInput.inputString += text;
		}
		private static void KeyCallback(IntPtr window,Keys key,int scanCode,KeyAction action,KeyModifiers mods)
		{
			if(action==KeyAction.Repeat) {
				return;
			}

			static void InputAction(Action<InputVariables> action)
			{
				action(fixedInput);
				action(renderInput);
			}

			switch(action) {
				case KeyAction.Press:
					InputAction(inputs => inputs.pressedKeys[key] = 0);
					break;
				case KeyAction.Release:
					InputAction(inputs => {
						if(inputs.pressedKeys.TryGetValue(key,out byte release)) {
							inputs.pressedKeys[key] = (byte)Math.Max(2,(int)release);
						}
					});
					break;
			}
		}
	}
}
