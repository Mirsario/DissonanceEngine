using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame.Inputs
{
	public class Sprint : SingletonInputTrigger
	{
		protected override void Init(out string name,out InputBinding[] bindings,out float minValue,out float maxValue)
		{
			name = "Sprint";

			minValue = 0f;
			maxValue = 1f;

			bindings = new InputBinding[] {
				"LShift",
				"GamePad0 Button6"
			};
		}
	}
}
