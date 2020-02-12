using Dissonance.Framework.GLFW3;
using System;

namespace Dissonance.Engine
{
	public class InputBinding
	{
		public enum BindingType
		{
			Unknown,
			Keyboard,
			Mouse,
			//Gamepad
		}

		public readonly BindingType Type;
		public readonly object Binding;

		public float deadZone;
		public float sensitivity;
		public bool inversed;

		public float Value {
			get {
				float value = RawValue;

				return (value>deadZone || value<-deadZone) ? (inversed ? -value : value)*sensitivity : 0f;
			}
		}
		public float RawValue => Type switch {
			BindingType.Keyboard => Input.GetKey((Keys)Binding) ? 1f : 0f,
			BindingType.Mouse => Input.GetMouseButton((MouseButton)Binding) ? 1f : 0f,
			_ => 0f
		};

		public InputBinding(Keys input,float sensitivity = 1f,float deadZone = 0.2f,bool inversed = false) : this(sensitivity,deadZone,inversed)
		{
			Type = BindingType.Keyboard;
			Binding = input;
		}
		public InputBinding(MouseButton input,float sensitivity = 1f,float deadZone = 0.2f,bool inversed = false) : this(sensitivity,deadZone,inversed)
		{
			Type = BindingType.Mouse;
			Binding = input;
		}
		public InputBinding(string input,float sensitivity = 1f,float deadZone = 0.2f) : this(sensitivity,deadZone,false)
		{
			if(string.IsNullOrWhiteSpace(input)) {
				throw new Exception("Input name cannot be empty.");
			}

			bool minus = input[0]=='-';

			if(minus || input[0]=='+') {
				if(minus) {
					inversed = true;
				}

				input = input.Substring(1);
			}

			if(Enum.TryParse(input,out Keys keyResult)) {
				Type = BindingType.Keyboard;
				Binding = keyResult;
				return;
			}

			if(Enum.TryParse(input,out MouseButton mouseResult)) {
				Type = BindingType.Mouse;
				Binding = mouseResult;
				return;
			}

			Type = BindingType.Unknown;

			//throw new ArgumentException($"Unable to parse input string '{input}' to a binding.");
		}

		protected InputBinding(float sensitivity = 1f,float deadZone = 0.2f,bool inversed = false)
		{
			this.sensitivity = sensitivity;
			this.deadZone = deadZone;
			this.inversed = inversed;
		}

		public static implicit operator InputBinding(MouseButton button) => new InputBinding(button);
		public static implicit operator InputBinding(Keys key) => new InputBinding(key);
		public static implicit operator InputBinding(string str) => new InputBinding(str);
	}
}