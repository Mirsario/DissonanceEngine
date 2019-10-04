using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame.Inputs
{
	public class LookY : SingletonInputTrigger
	{
		protected override void Init(out string name,out InputBinding[] bindings,out float minValue,out float maxValue)
		{
			name = "LookY";

			minValue = float.MinValue;
			maxValue = float.MaxValue;

			bindings = new InputBinding[] {
				"-A",
				"+D",
				"GamePad0 Axis0"
			};
		}
	}
}
