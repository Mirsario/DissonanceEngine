using System;
using Dissonance.Engine.Core;

namespace Dissonance.Engine.Input
{
	public class InputTrigger
	{
		internal struct SummInput
		{
			public bool isPressed;
			public bool wasPressed;
			public float analogInput;
			public float prevAnalogInput;
		}

		public const float DefaultMinValue = float.NegativeInfinity;
		public const float DefaultMaxValue = float.PositiveInfinity;

		public static int Count { get; internal set; }

		public string name = "InputTrigger";
		public float minValue = DefaultMinValue;
		public float maxValue = DefaultMaxValue;

		internal int bindingCount;
		internal InputBinding[] bindings;
		internal SummInput fixedInput;
		internal SummInput renderInput;

		public int Id { get; internal set; }

		public bool IsPressed => CurrentInput.isPressed;
		public bool WasPressed => CurrentInput.wasPressed;
		public bool JustPressed => CurrentInput.isPressed && !CurrentInput.wasPressed;
		public bool JustReleased => CurrentInput.wasPressed && !CurrentInput.isPressed;

		public float Value {
			get => CurrentInput.analogInput;
			internal set {
				CurrentInput.analogInput = Mathf.Clamp(value, minValue, maxValue);
				CurrentInput.isPressed = value != 0f;
			}
		}
		public InputBinding[] Bindings {
			set {
				if(value == null) {
					throw new ArgumentNullException();
				}

				bindingCount = value.Length;
				bindings = new InputBinding[bindingCount];

				for(int i = 0; i < bindingCount; i++) {
					bindings[i] = value[i];
				}
			}
		}

		internal ref SummInput CurrentInput => ref (Game.Instance?.preInitDone != false ? ref fixedInput : ref renderInput);

		internal InputTrigger() { }

		internal virtual void Init(int id, string name, InputBinding[] bindings, float minValue, float maxValue)
		{
			Id = id;

			this.name = name;
			this.minValue = minValue;
			this.maxValue = maxValue;

			Bindings = bindings;
		}
	}
}