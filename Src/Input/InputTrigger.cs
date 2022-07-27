using System;

namespace Dissonance.Engine.Input;

public class InputTrigger
{
	internal struct SummInput
	{
		public bool IsPressed;
		public bool WasPressed;
		public float AnalogInput;
		public float PrevAnalogInput;
	}

	public const float DefaultMinValue = float.NegativeInfinity;
	public const float DefaultMaxValue = float.PositiveInfinity;

	public static int Count { get; internal set; }

	internal int bindingCount;
	internal InputBinding[] bindings;
	internal SummInput fixedInput;
	internal SummInput renderInput;

	public string Name { get; set; } = "InputTrigger";
	public float MinValue { get; set; } = DefaultMinValue;
	public float MaxValue { get; set; } = DefaultMaxValue;
	public int Id { get; internal set; }

	public bool IsPressed => CurrentInput.IsPressed;
	public bool WasPressed => CurrentInput.WasPressed;
	public bool JustPressed => CurrentInput.IsPressed && !CurrentInput.WasPressed;
	public bool JustReleased => CurrentInput.WasPressed && !CurrentInput.IsPressed;

	public float Value {
		get => CurrentInput.AnalogInput;
		internal set {
			CurrentInput.AnalogInput = MathHelper.Clamp(value, MinValue, MaxValue);
			CurrentInput.IsPressed = value != 0f;
		}
	}

	public float PreviousValue {
		get => CurrentInput.PrevAnalogInput;
		internal set {
			CurrentInput.PrevAnalogInput = MathHelper.Clamp(value, MinValue, MaxValue);
			CurrentInput.WasPressed = value != 0f;
		}
	}

	public InputBinding[] Bindings {
		set {
			if (value == null) {
				throw new ArgumentNullException(nameof(value));
			}

			bindingCount = value.Length;
			bindings = new InputBinding[bindingCount];

			for (int i = 0; i < bindingCount; i++) {
				bindings[i] = value[i];
			}
		}
	}

	internal ref SummInput CurrentInput => ref (GameEngine.InFixedUpdate ? ref fixedInput : ref renderInput);

	internal InputTrigger() { }

	internal virtual void Init(int id, string name, InputBinding[] bindings, float minValue, float maxValue)
	{
		Id = id;

		Name = name;
		MinValue = minValue;
		MaxValue = maxValue;

		Bindings = bindings;
	}
}
