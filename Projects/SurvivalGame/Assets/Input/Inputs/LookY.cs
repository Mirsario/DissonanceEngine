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

			bindings = new[] {
				new InputBinding("Mouse Y",GameSettings.MouseSensitivity,0f),
				"GamePad0 Axis4",
				"-Up",
				"+Down"
			};
		}
	}
}
