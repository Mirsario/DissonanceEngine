using System;
using System.Text;
using Dissonance.Engine.Graphics;
using Silk.NET.GLFW;

namespace Dissonance.Engine.Input
{
	partial class InputEngine
	{
		internal static void CursorPositionCallback(double x, double y)
		{
			var value = new Vector2((float)x, (float)y) * (Screen.Size / Screen.WindowSize);

			fixedInput.MousePosition = value;
			renderInput.MousePosition = value;
		}

		private static void MouseButtonCallback(MouseButton button, InputAction action, KeyModifiers mods)
		{
			bool value = action == InputAction.Press;

			fixedInput.MouseButtons[(int)button] = value;
			renderInput.MouseButtons[(int)button] = value;
		}

		private static void ScrollCallback(double xOffset, double yOffset)
		{
			fixedInput.MouseWheel = (int)yOffset;
			renderInput.MouseWheel = (int)yOffset;
		}

		private static void CharCallback(uint codePoint)
		{
			byte[] bytes = BitConverter.GetBytes(codePoint);
			string text = Encoding.UTF32.GetString(bytes);

			fixedInput.InputString += text;
			renderInput.InputString += text;
		}

		private static void KeyCallback(Keys key, int scanCode, InputAction action, KeyModifiers mods)
		{
			if (action == InputAction.Repeat) {
				return;
			}

			static void RunInputAction(Action<InputVariables> action)
			{
				action(fixedInput);
				action(renderInput);
			}

			switch (action) {
				case InputAction.Press:
					RunInputAction(inputs => inputs.PressedKeys[key] = 0);
					break;
				case InputAction.Release:
					RunInputAction(inputs => {
						if (inputs.PressedKeys.TryGetValue(key, out byte release)) {
							inputs.PressedKeys[key] = (byte)Math.Max(2, (int)release);
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

			Glfw.Api.SetCursorPosCallback(windowHandle, MousePositionCallback);
			Glfw.Api.SetMouseButtonCallback(windowHandle, MouseButtonCallback);
			Glfw.Api.SetScrollCallback(windowHandle, MouseScrollCallback);
			Glfw.Api.SetKeyCallback(windowHandle, KeyCallback);
			Glfw.Api.SetCharCallback(windowHandle, KeyStringCallback);*/
		}
	}
}
