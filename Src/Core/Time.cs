using System;
using System.Diagnostics;
using Dissonance.Engine.Core.Modules;
using Dissonance.Framework.Windowing;

namespace Dissonance.Engine.Core
{
	public sealed class Time : EngineModule
	{
		private struct TimeData
		{
			//Time
			public float game;
			public float gameDelta;
			public float global;
			public float globalDelta;
			//Framerate
			public float ms;
			public float msTemp;
			public uint frameNum;
			public uint framerate;
			//Etc
			public uint updateCount;
		}

		internal static Time Instance => Game.Instance.GetModule<Time>();

		//Auto
		public static float GameTime => Game.IsFixedUpdate ? FixedGameTime : RenderGameTime;
		public static float GlobalTime => Game.IsFixedUpdate ? FixedGlobalTime : RenderGlobalTime;
		public static float DeltaTime => Game.IsFixedUpdate ? FixedDeltaTime : RenderDeltaTime; //It might be a bit weird that this isn't 2 'GameDeltaTime' and 'GlobalDeltaTime' properties.
		//Fixed Time
		public static float FixedGameTime => Instance.fixedTime.game;
		public static float FixedGlobalTime => Instance.fixedTime.global;
		public static float FixedDeltaTime => 1f/(float)TargetUpdateFrequency;
		public static uint FixedUpdateCount => Instance.fixedTime.updateCount;
		//Render Time
		public static float RenderGameTime => Instance.renderTime.game;
		public static float RenderGlobalTime => Instance.renderTime.global;
		public static float RenderDeltaTime => Instance.renderTime.gameDelta;
		public static uint RenderUpdateCount => Instance.renderTime.updateCount;
		//Fixed Framerate
		public static uint FixedFramerate => Instance.fixedTime.framerate;
		public static float FixedMs => Instance.fixedTime.ms;
		//Render Framerate
		public static uint RenderFramerate => Instance.renderTime.framerate;
		public static float RenderMs => Instance.renderTime.ms;

		//Time
		public static float TimeScale {
			get => Instance.timeScale;
			set {
				if(value<0f) {
					throw new Exception("TimeScale cannot be negative.");
				}

				Instance.timeScale = value;
			}
		}
		//Target Framerate
		public static double TargetUpdateFrequency {
			get => Instance.targetUpdateFrequency;
			set {
				if(value<=0) {
					throw new ArgumentOutOfRangeException(nameof(value),"Value has to be more than zero.");
				}

				Instance.targetUpdateFrequency = value;
			}
		}
		public static double TargetRenderFrequency {
			get => Instance.targetRenderFrequency;
			set {
				if(value<0) {
					throw new ArgumentOutOfRangeException(nameof(value),"Value has to be more than or equal to zero.");
				}

				Instance.targetRenderFrequency = value;
			}
		}

		//Target Framerate
		private double targetUpdateFrequency = 60;
		private double targetRenderFrequency = 0;
		//Time
		private float timeScale = 1f;
		private TimeData fixedTime;
		private TimeData fixedTimePrev;
		private TimeData renderTime;
		private TimeData renderTimePrev;
		//Framerate
		private Stopwatch fixedStopwatch;
		private Stopwatch renderStopwatch;

		//Initialization
		protected override void PreInit()
		{
			targetRenderFrequency = 60; //device.RefreshRate;
			targetUpdateFrequency = 60;
		}
		protected override void Init()
		{
			fixedStopwatch = new Stopwatch();
			renderStopwatch = new Stopwatch();
		}

		//Fixed Update
		[HookPosition(-100000)]
		protected override void PreFixedUpdate() => PreUpdate(ref fixedTime,ref fixedTimePrev,fixedStopwatch);

		[HookPosition(100000)]
		protected override void PostFixedUpdate() => PostUpdate(ref fixedTime,ref fixedTimePrev,fixedStopwatch);

		//Render Update
		[HookPosition(-100000)]
		protected override void PreRenderUpdate() => PreUpdate(ref renderTime,ref renderTimePrev,renderStopwatch);

		[HookPosition(100000)]
		protected override void PostRenderUpdate() => PostUpdate(ref renderTime,ref renderTimePrev,renderStopwatch);

		private void PreUpdate(ref TimeData currentTime,ref TimeData previousTime,Stopwatch stopwatch)
		{
			stopwatch.Restart();

			currentTime.global = (float)GLFW.GetTime();
			currentTime.globalDelta = currentTime.global-previousTime.global;
			currentTime.gameDelta = currentTime.globalDelta*timeScale;
			currentTime.game += currentTime.gameDelta;

			currentTime.updateCount++;
		}
		private void PostUpdate(ref TimeData currentTime,ref TimeData previousTime,Stopwatch stopwatch)
		{
			MeasureFPS(ref currentTime,previousTime,stopwatch);

			previousTime = currentTime;
		}
		private static void MeasureFPS(ref TimeData time,TimeData timePrev,Stopwatch stopwatch)
		{
			time.frameNum++;
			time.msTemp += stopwatch.ElapsedMilliseconds;

			if(Mathf.FloorToInt(time.global)>Mathf.FloorToInt(timePrev.global)) {
				time.framerate = time.frameNum;
				time.frameNum = 0;
				time.ms = time.msTemp/Math.Max(1,time.framerate);
				time.msTemp = 0f;
			}
		}
	}
}
