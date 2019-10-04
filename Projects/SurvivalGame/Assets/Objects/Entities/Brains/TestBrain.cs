using GameEngine;
using ImmersionFramework;

namespace SurvivalGame
{
	/*public class TestBrain : Brain
	{
		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if(Time.FixedUpdateCount%20==0 && Rand.Next(3)==0) {
				Direction = new Vector3(Rand.Range(-1f,1f),0f,Rand.Range(-1f,1f)); //Will be normalized
				LookDirection = Transform.Forward;

				this[GameInput.moveX].value = 0f;
				this[GameInput.moveY].value = 1f;
			}
			if(Time.FixedUpdateCount%20==0 && Rand.Next(3)==0) {
				this[GameInput.jump].ActivateFor(1);
			}
		}
	}*/
}