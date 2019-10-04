using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame.Inputs
{
	public class Crouch : SingletonInputTrigger
	{
		protected override void Init(out string name,out InputBinding[] bindings,out float minValue,out float maxValue)
		{
			name = "Crouch";

			minValue = 0f;
			maxValue = 1f;

			bindings = new InputBinding[] {
				"LControl",
				"GamePad0 Button8"
			};
		}
	}
}
