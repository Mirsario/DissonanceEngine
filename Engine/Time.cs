using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameEngine
{
	public static class Time
	{
		private static int _updateFrames;
		public static int UpdateFrames {
			get {
				return _updateFrames;
			}
			set {
				Game.window.TargetUpdateFrequency = value;
				_updateFrames = (int)Game.window.TargetUpdateFrequency;
			}
		}
		private static int _renderFrames;
		public static int RenderFrames {
			get {
				return _renderFrames;
			}
			set {
				Game.window.TargetRenderFrequency = value;
				_renderFrames = (int)Game.window.TargetUpdateFrequency;
			}
		}
		
		//Fixed time
		internal static float FixedTime				{ private set; get; }
		internal static float FixedTimePrev			{ private set; get; }
		internal static float FixedTimeReal			{ private set; get; }
		internal static float FixedTimeRealPrev		{ private set; get; }
		internal static float FixedDeltaTime		{ private set; get; }

		//Render time
		internal static float RenderTime			{ private set; get; }
		internal static float RenderTimePrev		{ private set; get; }
		internal static float RenderTimeReal		{ private set; get; }
		internal static float RenderTimeRealPrev	{ private set; get; }
		internal static float RenderDeltaTime		{ private set; get; }

		//Main
		public static float GameTime => Game.fixedUpdate ? FixedTime : RenderTime;
		public static float GlobalTime => Game.fixedUpdate ? FixedTimeReal : RenderTimeReal;
		public static float DeltaTime => Game.fixedUpdate ? FixedDeltaTime : RenderDeltaTime;

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
		
		internal static void Init()
		{
			
		}
		internal static void UpdateFixed(double newDelta)
		{
			newDelta = 1.0/Game.targetUpdates;
			
			FixedTimePrev = FixedTime;
			FixedTimeRealPrev = FixedTimeReal;

			FixedDeltaTime = (float)newDelta;
			FixedTimeReal += FixedDeltaTime;
			FixedDeltaTime *= TimeScale;
			FixedTime += FixedDeltaTime;
		}
		internal static void UpdateRender(double newDelta)
		{
			RenderTimePrev = RenderTime;
			RenderTimeRealPrev = RenderTimeReal;

			RenderDeltaTime = (float)newDelta;
			RenderTimeReal += RenderDeltaTime;
			RenderDeltaTime *= TimeScale;
			RenderTime += RenderDeltaTime;
		}
	}
}