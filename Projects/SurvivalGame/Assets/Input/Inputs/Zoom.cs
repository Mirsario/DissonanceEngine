using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame.Inputs
{
	public class Zoom : SingletonInputTrigger
	{
		protected override void Init(out string name,out InputBinding[] bindings,out float minValue,out float maxValue)
		{
			name = "Zoom";

			minValue = float.MinValue;
			maxValue = float.MaxValue;

			bindings = new InputBinding[] {
				"-GamePad0 Button12",
				"+GamePad0 Button11",
				"Mouse ScrollWheel"
			};
		}
	}
}
