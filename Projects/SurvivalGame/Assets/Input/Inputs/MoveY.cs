using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame.Inputs
{
	public class MoveY : SingletonInputTrigger
	{
		protected override void Init(out string name,out InputBinding[] bindings,out float minValue,out float maxValue)
		{
			name = "MoveY";

			minValue = -1f;
			maxValue = 1f;

			bindings = new InputBinding[] {
				"-S",
				"+W",
				"GamePad0 Axis1"
			};
		}
	}
}
