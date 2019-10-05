using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;
using ImmersionFramework;

namespace SurvivalGame
{
	public abstract class Brain : InputProxyEntity
	{
		protected override void OnDetachedFrom(PhysicalEntity entity)
		{
			base.OnDetachedFrom(entity);

			if(Netplay.isClient) {
				SoundInstance.Create($"Magic.ogg",Transform.Position);
			}
		}
		/*protected override void OnAttachedTo(PhysicalEntity entity)
		{
			base.OnAttachedTo(entity);

			if(entity is LivingEntity livingEntity) {
				livingEntity.Brain = this;
			}
		}
		protected override void OnDetachedFrom(PhysicalEntity entity)
		{
			base.OnDetachedFrom(entity);

			if(entity is LivingEntity livingEntity && livingEntity.Brain==this) {
				livingEntity.Brain = null;
			}
		}*/

		/*public bool isReady;
		public int numSignals;
		public BrainSignal[] signals;

		public Entity Entity { get; protected set; }
		public Vector3 LookDirection { get; protected set; }

		protected Vector3 Direction {
			set {
				if(value.x==0f && value.y==0f && value.z==0f) {
					throw new Exception("Direction cannot have all axes set to zero.");
				}

				Transform.Rotation = Quaternion.FromDirection(value.Normalized,Vector3.Up);
			}
		}

		public BrainSignal this[InputTrigger trigger] => signals[trigger.Id];

		public virtual void AttachTo(Entity mob)
		{
			Entity = mob;
			
			isReady = true;
		}

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
		}*/
	}
}
