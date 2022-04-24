using System;
using System.Diagnostics;

namespace Dissonance.Engine
{
	public sealed class Time : EngineModule
	{
		private struct TimeData
		{
			// Time
			public float Game;
			public float GameDelta;
			public float Global;
			public float GlobalDelta;
			// Framerate
			public float Ms;
			public float MsTemp;
			public uint FrameNum;
			public uint Framerate;
			// Etc
			public uint UpdateCount;
		}

		// Target Framerate
		private static double targetUpdateFrequency;
		private static double targetRenderFrequency;
		// Time
		private static float timeScale = 1f;
		private static TimeData fixedTime;
		private static TimeData fixedTimePrev;
		private static TimeData renderTime;
		private static TimeData renderTimePrev;
		// Framerate
		private static Stopwatch globalStopwatch;
		private static Stopwatch fixedStopwatch;
		private static Stopwatch renderStopwatch;

		// Auto
		public static float GameTime => GameEngine.InFixedUpdate ? FixedGameTime : RenderGameTime;
		public static float GlobalTime => GameEngine.InFixedUpdate ? FixedGlobalTime : RenderGlobalTime;
		public static float DeltaTime => GameEngine.InFixedUpdate ? FixedDeltaTime : RenderDeltaTime; // It might be a bit weird that this isn't 2 'GameDeltaTime' and 'GlobalDeltaTime' properties.
		// Fixed Time
		public static float FixedGameTime => fixedTime.Game;
		public static float FixedGlobalTime => fixedTime.Global;
		public static float FixedDeltaTime => 1f / (float)TargetUpdateFrequency;
		public static uint FixedUpdateCount => fixedTime.UpdateCount;
		// Render Time
		public static float RenderGameTime => renderTime.Game;
		public static float RenderGlobalTime => renderTime.Global;
		public static float RenderDeltaTime => renderTime.GameDelta;
		public static uint RenderUpdateCount => renderTime.UpdateCount;
		// Fixed Framerate
		public static uint FixedFramerate => fixedTime.Framerate;
		public static float FixedMs => fixedTime.Ms;
		// Render Framerate
		public static uint RenderFramerate => renderTime.Framerate;
		public static float RenderMs => renderTime.Ms;

		// Time
		public static float TimeScale {
			get => timeScale;
			set {
				if (value < 0f) {
					throw new Exception("TimeScale cannot be negative.");
				}

				timeScale = value;
			}
		}
		// Target Framerate
		public static double TargetUpdateFrequency {
			get => targetUpdateFrequency;
			set {
				if (value <= 0) {
					throw new ArgumentOutOfRangeException(nameof(value), "Value has to be more than zero.");
				}

				targetUpdateFrequency = value;
			}
		}
		public static double TargetRenderFrequency {
			get => targetRenderFrequency;
			set {
				if (value < 0) {
					throw new ArgumentOutOfRangeException(nameof(value), "Value has to be more than or equal to zero.");
				}

				targetRenderFrequency = value;
			}
		}

		// Initialization

		protected override void PreInit()
		{
			targetRenderFrequency = 0;
			targetUpdateFrequency = 60;
		}

		protected override void Init()
		{
			fixedStopwatch = new Stopwatch();
			renderStopwatch = new Stopwatch();
			globalStopwatch = new Stopwatch();

			globalStopwatch.Start();
		}

		// Fixed Update

		[HookPosition(int.MinValue)]
		protected override void PreFixedUpdate()
			=> PreUpdate(ref fixedTime, ref fixedTimePrev, fixedStopwatch);

		[HookPosition(int.MaxValue)]
		protected override void PostFixedUpdate()
			=> PostUpdate(ref fixedTime, ref fixedTimePrev, fixedStopwatch);

		// Render Update

		[HookPosition(int.MinValue)]
		protected override void PreRenderUpdate()
			=> PreUpdate(ref renderTime, ref renderTimePrev, renderStopwatch);

		[HookPosition(int.MaxValue)]
		protected override void PostRenderUpdate()
			=> PostUpdate(ref renderTime, ref renderTimePrev, renderStopwatch);

		private static void PreUpdate(ref TimeData currentTime, ref TimeData previousTime, Stopwatch stopwatch)
		{
			stopwatch.Restart();

			currentTime.Global = (float)globalStopwatch.Elapsed.TotalSeconds;
			currentTime.GlobalDelta = currentTime.Global - previousTime.Global;
			currentTime.GameDelta = currentTime.GlobalDelta * timeScale;
			currentTime.Game += currentTime.GameDelta;

			currentTime.UpdateCount++;
		}

		private static void PostUpdate(ref TimeData currentTime, ref TimeData previousTime, Stopwatch stopwatch)
		{
			MeasureFPS(ref currentTime, previousTime, stopwatch);

			previousTime = currentTime;
		}

		private static void MeasureFPS(ref TimeData time, TimeData timePrev, Stopwatch stopwatch)
		{
			time.FrameNum++;
			time.MsTemp += stopwatch.ElapsedMilliseconds;

			if (MathF.Floor(time.Global) > MathF.Floor(timePrev.Global)) {
				time.Framerate = time.FrameNum;
				time.FrameNum = 0;
				time.Ms = time.MsTemp / Math.Max(1, time.Framerate);
				time.MsTemp = 0f;
			}
		}
	}
}
