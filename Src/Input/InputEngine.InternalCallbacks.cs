using System;
using System.Text;
using Dissonance.Engine.Core;
using Dissonance.Engine.Graphics;
using Dissonance.Engine.Structures;
using Dissonance.Framework.Windowing;
using Dissonance.Framework.Windowing.Input;

namespace Dissonance.Engine.Input
{
	partial class InputEngine
	{
		internal static void MousePositionCallback(IntPtr window,double x,double y)
		{
			var value = new Vector2((float)x,(float)y)*(Screen.Size/Screen.WindowSize);

			fixedInput.mousePosition = value;
			renderInput.mousePosition = value;
		}
		private static void MouseButtonCallback(IntPtr window,MouseButton button,MouseAction action,KeyModifiers mods)
		{
			bool value = action==MouseAction.Press;

			fixedInput.mouseButtons[(int)button] = value;
			renderInput.mouseButtons[(int)button] = value;
		}
		private static void MouseScrollCallback(IntPtr window,double xOffset,double yOffset)
		{
			fixedInput.mouseWheel = (int)yOffset;
			renderInput.mouseWheel = (int)yOffset;
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

		private void InitCallbacks()
		{
			if(windowing==null) {
				return;
			}

			var windowHandle = windowing.WindowHandle;

			if(windowHandle==IntPtr.Zero) {
				return;
			}

			GLFW.SetCursorPosCallback(windowHandle,MousePositionCallback);
			GLFW.SetMouseButtonCallback(windowHandle,MouseButtonCallback);
			GLFW.SetScrollCallback(windowHandle,MouseScrollCallback);
			GLFW.SetKeyCallback(windowHandle,KeyCallback);
			GLFW.SetCharCallback(windowHandle,KeyStringCallback);
		}
	}
}
