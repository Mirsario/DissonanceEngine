/*using GameEngine;
using ImmersionFramework;

namespace SurvivalGame
{
	public class InputSignal : IInputSignal
	{
		public readonly string name;
		public readonly float minValue;
		public readonly float maxValue;

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
			protected set => prevValue = value ? 1f : 0f;
		}

		public InputSignal(InputTrigger trigger)
		{
			name = trigger.Name;
			minValue = trigger.MinValue;
			maxValue = trigger.MaxValue;
			inputTrigger = trigger;
		}

		public void ActivateFor(int numTicks)
		{
			value = 1f;
			resetTimer = numTicks;
		}
	}
}*/