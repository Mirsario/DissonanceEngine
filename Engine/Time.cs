using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameEngine
{
	public static class Time
	{
		internal static uint targetUpdateCount;
		public static uint TargetUpdateCount {
			get => targetUpdateCount;
			set {
				Game.window.TargetUpdateFrequency = value;
				targetUpdateCount = (uint)Game.window.TargetUpdateFrequency;
			}
		}
		internal static uint targetRenderCount;
		public static uint TargetRenderCount {
			get => targetRenderCount;
			set {
				Game.window.TargetRenderFrequency = value;
				targetRenderCount = (uint)Game.window.TargetRenderFrequency;
			}
		}
		
		//Fixed time
		internal static float fixedTime;
		internal static float fixedTimePrev;
		internal static float fixedTimeReal;
		internal static float fixedTimeRealPrev;
		internal static float fixedDeltaTime;
		internal static uint fixedUpdateCount;

		//Render time
		internal static float renderTime;
		internal static float renderTimePrev;
		internal static float renderTimeReal;
		internal static float renderTimeRealPrev;
		internal static float renderDeltaTime;
		internal static uint renderUpdateCount;

		//Main
		public static float GameTime => Game.fixedUpdate ? fixedTime : renderTime;
		public static float GlobalTime => Game.fixedUpdate ? fixedTimeReal : renderTimeReal;
		public static float DeltaTime => Game.fixedUpdate ? fixedDeltaTime : renderDeltaTime;
		public static uint FixedUpdateCount => fixedUpdateCount;
		public static uint RenderUpdateCount => renderUpdateCount;

		internal static float _timeScale = 1f;
		public static float TimeScale {
			get => _timeScale;
			set {
				if(value<0f) {
					throw new Exception("TimeScale cannot be negative.");
				}
				_timeScale = value;
			}
		}
		
		internal static void PreInit()
		{
			targetRenderCount = 0;
			targetUpdateCount = 60;
		}
		internal static void Init()
		{
			
		}
		internal static void UpdateFixed(double newDelta)
		{
			fixedTimePrev = fixedTime;
			fixedTimeRealPrev = fixedTimeReal;

			fixedDeltaTime = (float)newDelta;
			fixedTimeReal += fixedDeltaTime;
			fixedDeltaTime *= TimeScale;
			fixedTime += fixedDeltaTime;

			fixedUpdateCount++;
		}
		internal static void UpdateRender(double newDelta)
		{
			renderTimePrev = renderTime;
			renderTimeRealPrev = renderTimeReal;

			renderDeltaTime = (float)newDelta;
			renderTimeReal += renderDeltaTime;
			renderDeltaTime *= TimeScale;
			renderTime += renderDeltaTime;

			renderUpdateCount++;
		}
	}
}