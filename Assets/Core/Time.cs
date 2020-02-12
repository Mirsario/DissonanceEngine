using System;
using System.Diagnostics;

namespace Dissonance.Engine
{
	public static class Time
	{
		//Target Framerate
		internal static double targetUpdateFrequency = 60;
		internal static double targetRenderFrequency = 0;
		//Time
		internal static float timeScale = 1f;
		internal static float fixedTime; //Fixed time
		internal static float fixedTimePrev;
		internal static float fixedTimeReal;
		internal static float fixedTimeRealPrev;
		//internal static float fixedDeltaTime;
		internal static uint fixedUpdateCount;
		internal static float renderTime; //Render time
		internal static float renderTimePrev;
		internal static float renderTimeReal;
		internal static float renderTimeRealPrev;
		internal static float renderDeltaTime;
		internal static uint renderUpdateCount;

		//Framerate Measuring
		private static Stopwatch fixedStopwatch; //Fixed time
		private static float fixedMs;
		private static float fixedMsTemp;
		private static uint fixedFrame;
		private static uint fixedFPS;
		private static Stopwatch renderStopwatch; //Render time
		private static float renderMs;
		private static float renderMsTemp;
		private static uint renderFrame;
		private static uint renderFPS;

		//Time
		public static float GameTime => Game.fixedUpdate ? fixedTime : renderTime;
		public static float GlobalTime => Game.fixedUpdate ? fixedTimeReal : renderTimeReal;
		public static float DeltaTime => Game.fixedUpdate ? FixedDeltaTime : RenderDeltaTime;

		public static float FixedGameTime => fixedTime;
		public static float RenderGameTime => renderTime;
		public static float FixedGlobalTime => fixedTimeReal;
		public static float RenderGlobalTime => renderTimeReal;
		public static float FixedDeltaTime => 1f/(float)TargetUpdateFrequency;
		public static float RenderDeltaTime => renderDeltaTime;
		public static uint FixedUpdateCount => fixedUpdateCount;
		public static uint RenderUpdateCount => renderUpdateCount;
		//Framerate
		public static uint FixedFramerate => fixedFPS;
		public static float FixedMs => fixedMs;
		public static uint RenderFramerate => renderFPS;
		public static float RenderMs => renderMs;

		//Time
		public static float TimeScale {
			get => timeScale;
			set {
				if(value<0f) {
					throw new Exception("TimeScale cannot be negative.");
				}
				timeScale = value;
			}
		}
		//Target Framerate
		public static double TargetUpdateFrequency {
			get => targetUpdateFrequency;
			set {
				if(value<=0) {
					throw new ArgumentOutOfRangeException(nameof(value),"Value has to be more than zero.");
				}

				targetUpdateFrequency = value;

				//TODO: Reimplement
				/*if(Game.window!=null) {
					Game.window.TargetUpdateFrequency = targetUpdateFrequency;
				}*/
			}
		}
		public static double TargetRenderFrequency {
			get => targetRenderFrequency;
			set {
				if(value<0) {
					throw new ArgumentOutOfRangeException(nameof(value),"Value has to be more than or equal to zero.");
				}

				targetRenderFrequency = value;

				//TODO: Reimplement
				/*if(Game.window!=null) {
					Game.window.TargetRenderFrequency = targetRenderFrequency;
				}*/
			}
		}

		internal static void PreInit()
		{
			//var device = DisplayDevice.Default;

			targetRenderFrequency = 60; //device.RefreshRate;
			targetUpdateFrequency = 60;
		}
		internal static void Init()
		{
			fixedStopwatch = new Stopwatch();
			renderStopwatch = new Stopwatch();
		}
		internal static void PreFixedUpdate() => fixedStopwatch.Restart();
		internal static void PreRenderUpdate() => renderStopwatch.Restart();
		internal static void UpdateFixed(double newTime)
		{
			//Real time
			fixedTimeRealPrev = fixedTimeReal;
			fixedTimeReal = (float)newTime;

			//Game time
			fixedTimePrev = fixedTime;
			fixedTime += FixedDeltaTime;

			fixedUpdateCount++;
		}
		internal static void UpdateRender(double newTime)
		{
			//Real time
			renderTimeRealPrev = renderTimeReal;
			renderTimeReal = (float)newTime;

			//Delta
			renderDeltaTime = renderTimeReal-renderTimeRealPrev;

			//Game time
			renderTimePrev = renderTime;
			renderTime += renderDeltaTime;

			renderUpdateCount++;
		}
		internal static void PostFixedUpdate() => MeasureFPS(ref fixedFPS,ref fixedFrame,fixedTime,fixedTimePrev,fixedStopwatch,ref fixedMs,ref fixedMsTemp);
		internal static void PostRenderUpdate() => MeasureFPS(ref renderFPS,ref renderFrame,renderTime,renderTimePrev,renderStopwatch,ref renderMs,ref renderMsTemp);

		private static void MeasureFPS(ref uint fps,ref uint frames,float time,float timePrev,Stopwatch stopwatch,ref float ms,ref float msTemp) //Move this somewhere
		{
			frames++;
			msTemp += stopwatch.ElapsedMilliseconds;

			if(Mathf.FloorToInt(time)>Mathf.FloorToInt(timePrev)) {
				fps = frames;
				frames = 0;
				ms = msTemp/Math.Max(1,fps);
				msTemp = 0f;
			}
		}
	}
}
 