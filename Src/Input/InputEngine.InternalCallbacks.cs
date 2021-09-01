using System;
using System.Text;
using Dissonance.Engine.Graphics;
using Dissonance.Framework.Windowing.Input;

namespace Dissonance.Engine.Input
{
	partial class InputEngine
	{
		internal static void CursorPositionCallback(double x, double y)
		{
			var value = new Vector2((float)x, (float)y) * (Screen.Size / Screen.WindowSize);

			fixedInput.mousePosition = value;
			renderInput.mousePosition = value;
		}

		private static void MouseButtonCallback(MouseButton button, MouseAction action, KeyModifiers mods)
		{
			bool value = action == MouseAction.Press;

			fixedInput.mouseButtons[(int)button] = value;
			renderInput.mouseButtons[(int)button] = value;
		}

		private static void ScrollCallback(double xOffset, double yOffset)
		{
			fixedInput.mouseWheel = (int)yOffset;
			renderInput.mouseWheel = (int)yOffset;
		}

		private static void CharCallback(uint codePoint)
		{
			byte[] bytes = BitConverter.GetBytes(codePoint);
			string text = Encoding.UTF32.GetString(bytes);

			fixedInput.inputString += text;
			renderInput.inputString += text;
		}

		private static void KeyCallback(Keys key, int scanCode, KeyAction action, KeyModifiers mods)
		{
			if (action == KeyAction.Repeat) {
				return;
			}

			void InputAction(Action<InputVariables> action)
			{
				action(fixedInput);
				action(renderInput);
			}

			switch (action) {
				case KeyAction.Press:
					InputAction(inputs => inputs.pressedKeys[key] = 0);
					break;
				case KeyAction.Release:
					InputAction(inputs => {
						if (inputs.pressedKeys.TryGetValue(key, out byte release)) {
							inputs.pressedKeys[key] = (byte)Math.Max(2, (int)release);
						}
					});
					break;
			}
		}

		private static void InitCallbacks()
		{
			if (windowing == null) {
				return;
			}

			windowing.OnCursorPositionCallback += CursorPositionCallback;
			windowing.OnMouseButtonCallback += MouseButtonCallback;
			windowing.OnScrollCallback += ScrollCallback;
			windowing.OnKeyCallback += KeyCallback;
			windowing.OnCharCallback += CharCallback;

			/*var windowHandle = windowing.WindowHandle;

			if (windowHandle == IntPtr.Zero) {
				return;
			}

			GLFW.SetCursorPosCallback(windowHandle, MousePositionCallback);
			GLFW.SetMouseButtonCallback(windowHandle, MouseButtonCallback);
			GLFW.SetScrollCallback(windowHandle, MouseScrollCallback);
			GLFW.SetKeyCallback(windowHandle, KeyCallback);
			GLFW.SetCharCallback(windowHandle, KeyStringCallback);*/
		}
	}
}
