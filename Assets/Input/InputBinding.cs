using System;

namespace GameEngine
{
	public class InputBinding
	{
		public readonly string Input;
		public readonly string InputLower;
		public readonly int InputHash;

		public float deadZone;
		public float sensitivity;
		public bool inversed;
		internal float fixedAnalogInput;
		internal float renderAnalogInput;
		internal float prevFixedAnalogInput;
		internal float prevRenderAnalogInput;

		public InputBinding(string input,float sensitivity = 1f,float deadZone = 0.2f)
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

			Input = input;
			InputLower = input.ToLower();
			InputHash = InputLower.GetHashCode();

			this.sensitivity = sensitivity;
			this.deadZone = deadZone;
		}

		public static implicit operator InputBinding(MouseButton button) => new InputBinding("Mouse "+button);
		public static implicit operator InputBinding(Keys key) => new InputBinding(key.ToString());
		public static implicit operator InputBinding(string str) => new InputBinding(str);
	}
}