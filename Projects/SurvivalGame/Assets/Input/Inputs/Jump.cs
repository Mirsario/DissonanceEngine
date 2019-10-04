using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame.Inputs
{
	public class Jump : SingletonInputTrigger
	{
		protected override void Init(out string name,out InputBinding[] bindings,out float minValue,out float maxValue)
		{
			name = "Jump";

			minValue = 0f;
			maxValue = 1f;

			bindings = new InputBinding[] {
				"Space",
				"GamePad0 Button0",
				"GamePad0 Button7"
			};
		}
	}
}
