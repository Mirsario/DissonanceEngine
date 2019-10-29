using AbyssCrusaders.Core;
using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders.Content.Entities
{
	public partial class Player
	{
		public bool isDodging;
		public float dodgerollProgress;

		private float lastDodgerollTime;
		private bool dodgerollLanded;

		private float DodgerollTime => 1.25f;
		private float DodgerollCooldown => 1f;

		public void UpdateDodgerolls(bool renderTime)
		{
			if(isDodging) {
				float prevProgress = dodgerollProgress;

				if(!renderTime) {
					const float FallPoint = 0.4f;

					if(prevProgress>=FallPoint && !dodgerollLanded && collisions.down) {
						dodgerollLanded = true;

						if(Netplay.isClient) {
							Footstep("Land",0.5f);

							SoundInstance.Create($"Dodgeroll{1+Rand.Next(3)}.ogg",Position);
						}
					}
					if(prevProgress<FallPoint || dodgerollLanded) {
						dodgerollProgress += Time.FixedDeltaTime/DodgerollTime;
					}

					if(dodgerollProgress>=1f) {
						isDodging = false;

						if(Netplay.isClient) {
							Footstep("Walk",0.5f);
						}
					} else {
						float rollSpeed = dodgerollProgress<0.11f ? 4f : (dodgerollProgress>0.65f ? 4f : 16f);

						velocity.x = direction>0 ? Mathf.Max(velocity.x,rollSpeed) : Mathf.Min(velocity.x,-rollSpeed);

						autoStepDown = false;
						moveInput = Vector2.Zero;

						if(dodgerollProgress>=0.11f && prevProgress<0.11f) {
							velocity.y -= 13f;

							if(Netplay.isClient) {
								Footstep("Run");
							}
						}
					}
				}
			}

			if(!renderTime && !isDodging && collisions.down && GameInput.dodgeroll.IsPressed && Time.GameTime-lastDodgerollTime>DodgerollTime+DodgerollCooldown) {
				isDodging = true;

				if(moveInput.y>0f) {
					dodgerollProgress = 0.4f;
					dodgerollLanded = true;

					if(Netplay.isClient) {
						SoundInstance.Create($"Dodgeroll{1+Rand.Next(3)}.ogg",Position);
					}
				} else {
					dodgerollProgress = 0.0f;
					dodgerollLanded = false;
				}

				lastDodgerollTime = Time.GameTime;
			}
		}
	}
}