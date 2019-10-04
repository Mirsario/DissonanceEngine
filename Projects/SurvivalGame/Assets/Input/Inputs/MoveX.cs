using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame.Inputs
{
	public class MoveX : SingletonInputTrigger
	{
		protected override void Init(out string name,out InputBinding[] bindings,out float minValue,out float maxValue)
		{
			name = "MoveX";

			minValue = -1f;
			maxValue = 1f;

			bindings = new InputBinding[] {
				"-A",
				"+D",
				"GamePad0 Axis0"
			};
		}
	}
}
