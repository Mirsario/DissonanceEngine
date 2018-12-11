using System.Collections.Generic;
using GameEngine;

namespace SurvivalGame
{
	public class ScreenShake
	{
		public static List<ScreenShake> screenShakes = new List<ScreenShake>();

		public Vector3? position;
		public readonly float timeMax;
		public float time;
		public float power;
		public float distance;

		public ScreenShake(float power,float time,float distance,Vector3? position = null)
		{
			this.power = power;
			this.time = timeMax = time;
			this.position = position;
			this.distance = distance;
		}

		public void Update()
		{
			time -= Time.DeltaTime;
			if(time<0f) {
				screenShakes.Remove(this);
			}
		}

		public static void StaticRenderUpdate()
		{
			for(int i = 0;i<screenShakes.Count;i++) {
				screenShakes[i].Update();
			}
		}
		public static ScreenShake New(float power,float time,float distance,Vector3? position = null)
		{
			var screenShake = new ScreenShake(power,time,distance,position);
			screenShakes.Add(screenShake);
			return screenShake;
		}
		public static float GetPowerAtPoint(Vector3 point)
		{
			float power = 0f;
			for(int i = 0;i<screenShakes.Count;i++) {
				var shake = screenShakes[i];
				float maxPower;
				if(shake.position.HasValue) {
					maxPower = (1f-Mathf.Min(1f,Vector3.Distance(shake.position.Value,point)/shake.distance))*shake.power;
				}else{
					maxPower = shake.power;
				}
				power += Mathf.Lerp(0f,maxPower,shake.time/shake.timeMax);
			}
			return power;
		}
	}
}