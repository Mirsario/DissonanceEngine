using GameEngine;

namespace Game
{
	public class BrainSignal
	{
		public readonly string name;
		public readonly float minValue;
		public readonly float maxValue;
		public InputTrigger inputTrigger;
		public float value;
		public float prevValue;
		public int resetTimer;

		public bool Active {
			get => value>0f;
			set => this.value = value ? 1f : 0f;
		}
		public bool WasActive {
			get => prevValue>0f;
			set => prevValue = value ? 1f : 0f;
		}
		public bool JustActivated => Active && !WasActive;
		public bool JustDeactivated => WasActive && !Active;

		public BrainSignal(InputTrigger trigger)
		{
			name = trigger.name;
			minValue = trigger.minValue;
			maxValue = trigger.maxValue;
			inputTrigger = trigger;
		}

		public void ActivateFor(int numTicks)
		{
			value = 1f;
			resetTimer = numTicks;
		}
	}
}