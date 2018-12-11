using GameEngine;

namespace Game
{
	public class PlayerBrain : Brain
	{
		public override void FixedUpdate()
		{
			for(int i = 0;i<numSignals;i++) {
				var signal = signals[i];
				signal.prevValue = signal.value;
				signal.value = signal.inputTrigger.Value;
			}
		}
		public override void RenderUpdate()
		{
			Transform.Rotation = Main.camera.Transform.Rotation;
			LookDirection = Main.camera.Transform.Forward;
		}
	}
}