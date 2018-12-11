using System;
using GameEngine;

namespace Game
{
	public abstract class Brain : GameObject
	{
		public int numSignals;
		public BrainSignal[] signals;
		public bool isReady;

		public Mob Mob { get; protected set; }
		public Vector3 LookDirection { get; protected set; }
		protected Vector3 Direction {
			set {
				if(value.x==0f && value.y==0f && value.z==0f) {
					throw new Exception("Direction cannot have all axes set to zero.");
				}
				Transform.Rotation = Quaternion.FromDirection(value.Normalized,Vector3.up);
			}
		}

		public BrainSignal this[InputTrigger trigger] => signals[trigger.id];

		public override void OnInit()
		{
			var triggers = Input.Triggers;
			numSignals = triggers.Length;
			signals = new BrainSignal[numSignals];
			for(int i = 0;i<numSignals;i++) {
				signals[i] = new BrainSignal(triggers[i]);
			}
		}
		public override void FixedUpdate()
		{
			for(int i = 0;i<numSignals;i++) {
				var signal = signals[i];
				signal.prevValue = signal.value;

				if(signal.resetTimer>0 && --signal.resetTimer==0) {
					signal.value = 0f;
				}
			}
		}

		public virtual void AttachTo(Mob mob)
		{
			Mob = mob;
			//Transform.parent = mob.Transform;
			//Transform.LocalPosition = Vector3.zero;
			isReady = true;
		}
	}
	public static class BrainExtensions
	{
		public static float Signal(this Brain brain,InputTrigger trigger) => brain?[trigger].value ?? 0f;
		public static Vector2 Signal2(this Brain brain,InputTrigger xTrigger,InputTrigger yTrigger) => brain==null ? default : new Vector2(brain[xTrigger].value,brain[yTrigger].value);
		public static bool Active(this Brain brain,InputTrigger trigger) => brain?[trigger].Active ?? false;
		public static bool WasActive(this Brain brain,InputTrigger trigger) => brain?[trigger].WasActive ?? false;
		public static bool JustActivated(this Brain brain,InputTrigger trigger) => brain?[trigger].JustActivated ?? false;
		public static bool JustDeactivated(this Brain brain,InputTrigger trigger) => brain?[trigger].JustDeactivated ?? false;
	}
}