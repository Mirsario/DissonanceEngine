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
		
		public readonly int id;
		public readonly string name;
		public float maxValue;
		public float minValue;

		internal int bindingCount;
		internal InputBinding[] bindings;
		internal int[] bindingsHashes;
		internal SummInput fixedInput;
		internal SummInput renderInput;

		internal InputBinding[] Bindings {
			set {
				if(value==null) {
					throw new ArgumentNullException();
				}
				bindingCount = value.Length;
				bindingsHashes = new int[bindingCount];
				bindings = new InputBinding[bindingCount];
				for(int i = 0;i<bindingCount;i++) {
					var binding = value[i];
					bindings[i] = binding;
					bindingsHashes[i] = binding.inputHash;
				}
			}
		}
		public float Value => Game.fixedUpdate ? fixedInput.analogInput : renderInput.analogInput;
		public bool IsPressed => Game.fixedUpdate ? fixedInput.isPressed : renderInput.isPressed;
		public bool WasPressed => Game.fixedUpdate ? fixedInput.wasPressed : renderInput.wasPressed;
		public bool JustPressed => Game.fixedUpdate ? (fixedInput.isPressed && !fixedInput.wasPressed) : (renderInput.isPressed && !renderInput.wasPressed);
		public bool JustReleased => Game.fixedUpdate ? (fixedInput.wasPressed && !fixedInput.isPressed) : (renderInput.wasPressed && !renderInput.isPressed);

		internal InputTrigger(int id,string name,InputBinding[] bindings,float minValue,float maxValue)
		{
			this.id = id;
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