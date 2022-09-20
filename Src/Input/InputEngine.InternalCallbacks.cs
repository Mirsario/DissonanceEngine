using System;
using System.Text;
using Silk.NET.GLFW;
using GlfwKeys = Silk.NET.GLFW.Keys;
using GlfwMouseButton = Silk.NET.GLFW.MouseButton;

namespace Dissonance.Engine.Input;

partial class InputEngine
{
	private static void MouseButtonCallback(GlfwMouseButton button, InputAction action, KeyModifiers mods)
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

	private static void KeyCallback(GlfwKeys glfwKey, int scanCode, InputAction action, KeyModifiers mods)
	{
		if (action == InputAction.Repeat) {
			return;
		}

		static void RunInputAction(Action<InputVariables> action)
		{
			action(fixedInput);
			action(renderInput);
		}

		var key = (Keys)glfwKey;

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

		windowing.OnMouseButtonCallback += MouseButtonCallback;
		windowing.OnScrollCallback += ScrollCallback;
		windowing.OnKeyCallback += KeyCallback;
		windowing.OnCharCallback += CharCallback;
	}
}
