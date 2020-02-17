using System;
using Dissonance.Framework.Windowing.Input;

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

		public readonly string Binding;

		public float deadZone;
		public float sensitivity;
		public bool inversed;

		public float Value {
			get {
				float value = RawValue;

				return (value>deadZone || value<-deadZone) ? (inversed ? -value : value)*sensitivity : 0f;
			}
		}
		public float RawValue => Input.GetSignal(Binding);

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

			Binding = input;
		}

		protected InputBinding(float sensitivity = 1f,float deadZone = 0.2f,bool inversed = false)
		{
			this.sensitivity = sensitivity;
			this.deadZone = deadZone;
			this.inversed = inversed;
		}

		public static implicit operator InputBinding(MouseButton button) => new InputBinding($"Mouse{button}");
		public static implicit operator InputBinding(Keys key) => new InputBinding(key.ToString());
		public static implicit operator InputBinding(string str) => new InputBinding(str);
	}
}