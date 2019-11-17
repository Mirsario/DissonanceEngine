using System;

namespace GameEngine
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

		public float Value => Game.fixedUpdate ? fixedInput.analogInput : renderInput.analogInput;
		public bool IsPressed => Game.fixedUpdate ? fixedInput.isPressed : renderInput.isPressed;
		public bool WasPressed => Game.fixedUpdate ? fixedInput.wasPressed : renderInput.wasPressed;
		public bool JustPressed => Game.fixedUpdate ? (fixedInput.isPressed && !fixedInput.wasPressed) : (renderInput.isPressed && !renderInput.wasPressed);
		public bool JustReleased => Game.fixedUpdate ? (fixedInput.wasPressed && !fixedInput.isPressed) : (renderInput.wasPressed && !renderInput.isPressed);

		public InputBinding[] Bindings {
			set {
				if(value==null) {
					throw new ArgumentNullException();
				}

				bindingCount = value.Length;
				bindings = new InputBinding[bindingCount];

				for(int i = 0;i<bindingCount;i++) {
					bindings[i] = value[i];
				}
			}
		}

		internal int id;
		public int Id {
			get => id;
			internal set => id = value;
		}

		internal InputTrigger() {}

		internal virtual void Init(int id,string name,InputBinding[] bindings,float minValue,float maxValue)
		{
			Id = id;

			this.name = name;
			this.minValue = minValue;
			this.maxValue = maxValue;

			Bindings = bindings;
		}

		internal void SetAnalogValue(float fixedValue,float renderValue)
		{
			fixedInput.analogInput = fixedValue<minValue ? minValue : (fixedValue>maxValue ? maxValue : fixedValue);
			fixedInput.isPressed = fixedInput.analogInput!=0f;

			renderInput.analogInput = renderValue<minValue ? minValue : (renderValue>maxValue ? maxValue : renderValue);
			renderInput.isPressed = renderInput.analogInput!=0f;
		}
	}
}