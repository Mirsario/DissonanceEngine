using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;
using ImmersionFramework;
using SurvivalGame.Inputs;

namespace SurvivalGame
{
	public abstract class Soul : InputProxyEntity
	{
		public override Type CameraControllerType => typeof(FirstPersonCamera);

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if(IsAttached) {
				return;
			}

			var moveInput = this.Value<MoveX,MoveY>();

			if(this.Active<SecondaryUse>()) {
				velocity = Vector3.Zero;
			} else {
				Vector3 goalVelocity = Vector3.Zero;
				goalVelocity += Transform.Forward*this.Value<MoveY>();
				goalVelocity += Transform.Right*this.Value<MoveX>();
				goalVelocity += Transform.Up*(this.Value<Jump>()-this.Value<Crouch>());
				goalVelocity = goalVelocity.Normalized*0.25f;

				velocity = Vector3.Lerp(velocity,goalVelocity,Time.FixedDeltaTime*4f);
			}

			Transform.Position += velocity;
		}
	}
}
