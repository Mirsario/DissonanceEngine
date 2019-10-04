using GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class EulerDirectionTest : GameObject
	{
		public Vector3 originalRotation;

		public override void OnInit()
		{
			Console.Clear();
		}
		public override void FixedUpdate()
		{
			originalRotation.x += (Input.GetKeyDown(Keys.Down) ? 10f : 0f)-(Input.GetKeyDown(Keys.Up) ? 10f : 0f);
			originalRotation.y += (Input.GetKeyDown(Keys.Right) ? 10f : 0f)-(Input.GetKeyDown(Keys.Left) ? 10f : 0f);
			//originalRotation = Vector3.Repeat(originalRotation,360f);

			Console.SetCursorPosition(0,0);

			//Test
			Debug.Log("=====START=====");
			Debug.Log("");

			var resultDirection = Vector3.EulerToDirection(originalRotation);
			var resultRotation = Vector3.DirectionToEuler(resultDirection);
			Debug.Log($"{nameof(originalRotation)}: \t{originalRotation}");
			Debug.Log($"{nameof(resultDirection)}: \t{resultDirection}");
			Debug.Log($"{nameof(resultRotation)}: \t{resultRotation}");

			Debug.Log("");
			Debug.Log("=====END=====");
		}
	}
}
