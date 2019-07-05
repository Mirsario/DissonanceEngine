using System;
using System.Diagnostics;

namespace GameEngine
{
	public static class Time
	{
		internal static uint targetUpdateCount;
		public static uint TargetUpdateCount {
			get => targetUpdateCount;
			set {
				targetUpdateCount = value;

				if(Game.window!=null) {
					Game.window.TargetUpdateFrequency = value;
				}
			}
		}
		internal static uint targetRenderCount;
		public static uint TargetRenderCount {
			get => targetRenderCount;
			set {
				targetRenderCount = value;

				if(Game.window!=null) {
					Game.window.TargetRenderFrequency = targetRenderCount;
				}
			}
		}
		
		//Time
		internal static float fixedTime; //Fixed time
		internal static float fixedTimePrev;
		internal static float fixedTimeReal;
		internal static float fixedTimeRealPrev;
		internal static float fixedDeltaTime;
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

		public static float GameTime => Game.fixedUpdate ? fixedTime : renderTime;
		public static float GlobalTime => Game.fixedUpdate ? fixedTimeReal : renderTimeReal;
		public static float DeltaTime => Game.fixedUpdate ? fixedDeltaTime : renderDeltaTime;
		public static float FixedDeltaTime => fixedDeltaTime;
		public static float RenderDeltaTime => renderDeltaTime;
		public static uint FixedUpdateCount => fixedUpdateCount;
		public static uint RenderUpdateCount => renderUpdateCount;
		//Framerate
		public static uint LogicFramerate => fixedFPS;
		public static float LogicMs => fixedMs;
		public static uint RenderFramerate => renderFPS;
		public static float RenderMs => renderMs;

		internal static float timeScale = 1f;
		public static float TimeScale {
			get => timeScale;
			set {
				if(value<0f) {
					throw new Exception("TimeScale cannot be negative.");
				}
				timeScale = value;
			}
		}
		
		internal static void PreInit()
		{
			targetRenderCount = 0;
			targetUpdateCount = 60;
		}
		internal static void Init()
		{
			fixedStopwatch = new Stopwatch();
			renderStopwatch = new Stopwatch();
		}
		internal static void PreFixedUpdate() => fixedStopwatch.Restart();
		internal static void PreRenderUpdate() => renderStopwatch.Restart();
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