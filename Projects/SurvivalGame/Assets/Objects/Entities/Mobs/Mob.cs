using System;
using GameEngine;

namespace SurvivalGame
{
	public class Mob : Entity
	{
		public Vector3 velocity;
		public Brain brain;
		private Brain brainBackup;

		public virtual Type BrainType => typeof(TestBrain);

		public override void UpdateIsPlayer(bool isPlayer)
		{
			base.UpdateIsPlayer(isPlayer);
			if(isPlayer) {
				if(brain!=null && !(brain is PlayerBrain)) {
					brainBackup = brain;
				}
				brain = Instantiate<PlayerBrain>();
			}else{
				if(brain is PlayerBrain && brainBackup!=null) {
					brain.Dispose();
					brain = brainBackup;
					brainBackup = null;
				}else{
					brain = (Brain)Instantiate(BrainType);
				}
			}
			brain?.AttachTo(this);
		}
	}
}

