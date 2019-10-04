using GameEngine;

namespace ImmersionFramework
{
	public struct InputSignal : IInputSignal
	{
		public readonly string Name;
		public readonly float MinValue;
		public readonly float MaxValue;

		public InputTrigger inputTrigger;
		public float value;
		public float prevValue;
		public int resetTimer;

		public bool JustActivated => Active && !WasActive;
		public bool JustDeactivated => WasActive && !Active;

		public bool Active {
			get => value>0f;
			set => this.value = value ? 1f : 0f;
		}
		public bool WasActive {
			get => prevValue>0f;
			private set => prevValue = value ? 1f : 0f;
		}

		public InputSignal(InputTrigger trigger)
		{
			Name = trigger.name;
			MinValue = trigger.minValue;
			MaxValue = trigger.maxValue;

			inputTrigger = trigger;

			value = prevValue = 0f;
			resetTimer = 0;
		}

		public void ActivateFor(int numTicks)
		{
			value = 1f;
			resetTimer = numTicks;
		}
	}
}