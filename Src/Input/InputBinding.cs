using System;
using Silk.NET.GLFW;

namespace Dissonance.Engine.Input
{
	public class InputBinding
	{
		public enum BindingType
		{
			Unknown,
			Keyboard,
			Mouse,
			// Gamepad
		}

		public string Binding { get; }
		public float DeadZone { get; set; }
		public float Sensitivity { get; set; }
		public bool Inversed { get; set; }

		public float RawValue => InputEngine.GetSignal(Binding);

		public float Value {
			get {
				float value = RawValue;

				return value > DeadZone || value < -DeadZone ? (Inversed ? -value : value) * Sensitivity : 0f;
			}
		}

		public InputBinding(string input, float sensitivity = 1f, float deadZone = 0.2f) : this(sensitivity, deadZone, false)
		{
			if (string.IsNullOrWhiteSpace(input)) {
				throw new Exception("Input name cannot be empty.");
			}

			bool minus = input[0] == '-';

			if (minus || input[0] == '+') {
				if (minus) {
					Inversed = true;
				}

				input = input.Substring(1);
			}

			Binding = input;
		}

		protected InputBinding(float sensitivity = 1f, float deadZone = 0.2f, bool inversed = false)
		{
			Sensitivity = sensitivity;
			DeadZone = deadZone;
			Inversed = inversed;
		}

		public static implicit operator InputBinding(MouseButton button)
			=> new($"Mouse{button}");

		public static implicit operator InputBinding(Keys key)
			=> new(key.ToString());

		public static implicit operator InputBinding(string str)
			=> new(str);
	}
}
