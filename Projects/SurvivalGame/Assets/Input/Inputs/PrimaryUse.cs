using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame.Inputs
{
	public class PrimaryUse : SingletonInputTrigger
	{
		protected override void Init(out string name,out InputBinding[] bindings,out float minValue,out float maxValue)
		{
			name = "PrimaryUse";

			minValue = 0f;
			maxValue = 1f;

			bindings = new InputBinding[] {
				MouseButton.Left,
				new InputBinding("GamePad0 Axis5",deadZone:0.5f)
			};
		}
	}
}
