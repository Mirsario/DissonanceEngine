using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Dissonance.Engine.Graphics;

namespace Dissonance.Engine
{
	//TODO: Add proper toggling between fullscreen, windowed fullscreen and normal windowed modes
	public partial class Game : IDisposable
	{
		private static volatile Game instance;

		public static bool HasFocus { get; internal set; } = true;

		/// <summary>
		/// The current running game instance. Will be null before <see cref="Run(GameFlags, string[])"/> is called.
		/// </summary>
		public static Game Instance => instance ?? throw new InvalidOperationException($"No active Game instance currently exists.");

		internal bool shouldQuit;
		internal bool fixedUpdate;

		private Stopwatch updateStopwatch;
		private Stopwatch renderStopwatch;
		private ulong numFixedUpdates;

		/// <summary>
		/// Unused.
		/// </summary>
		public string Name { get; set; } = "UntitledGame";

		/// <summary>
		/// The window title. Set this in the constructor of your game.
		/// </summary>
		public string DisplayName { get; set; } = "Untitled Game";

		public IReadOnlyList<string> StartArguments { get; private set; }

		/// <summary>
		/// Called before the game is initialized. Use <see cref="Rendering.SetRenderingPipeline{T}"/> here.
		/// </summary>
		public virtual void PreInit() { }

		/// <summary>
		/// Called when the game starts.
		/// </summary>
		public virtual void Start() { }

		/// <summary>
		/// Called when render passes are rendered.
		/// </summary>
		public virtual void OnGUI() { }

		/// <summary>
		/// Called when the game instance is disposed.
		/// </summary>
		public virtual void OnDispose() { }

		/// <summary>
		/// Begins running the game.
		/// </summary>
		/// <param name="flags">Flags describing how the game should be run.</param>
		public void Run(GameFlags flags = GameFlags.None, string[] args = null)
		{
			if (instance != null) {
				throw new InvalidOperationException("Cannot run a game while one instance is already running. If you wish to run multiple game instances - use AssemblyLoadContexts to isolate engine & same game assemblies from each other.");
			}

			instance = this;
			StartArguments = Array.AsReadOnly(args ?? Array.Empty<string>());

			AppDomain.CurrentDomain.ProcessExit += ApplicationQuit;

			Debug.Log($"Working directory: '{Directory.GetCurrentDirectory()}'.");

			PreInit();

			// Initialization of the assembly will be delayed, but registration has to be done before engine initialization.
			AssemblyManagement.RegisterAssembly(GetType().Assembly);
			GameEngine.Initialize(flags);

			Init();

			if (!GameEngine.Flags.HasFlag(GameFlags.ManualUpdate)) {
				UpdateLoop();
				Dispose();
			}
		}

		public void Dispose()
		{
			OnDispose();

			GameEngine.Terminate();

			if (instance == this) {
				instance = null;
			}

			GC.SuppressFinalize(this);
		}

		public void Update()
		{
			if (updateStopwatch == null) {
				updateStopwatch = new Stopwatch();

				updateStopwatch.Start();
			}

			while (numFixedUpdates == 0 || numFixedUpdates < (ulong)Math.Floor(updateStopwatch.Elapsed.TotalSeconds * Time.TargetUpdateFrequency)) {
				FixedUpdateInternal();

				numFixedUpdates++;
			}

			if (GameEngine.Flags.HasFlag(GameFlags.NoGraphics)) {
				Thread.Sleep(1);
				return;
			}

			renderStopwatch ??= new Stopwatch();

			RenderUpdateInternal();

			if (Time.TargetRenderFrequency > 0) {
				double targetMs = 1000.0 / Time.TargetRenderFrequency;

				TimeSpan timeToSleep = TimeSpan.FromMilliseconds(targetMs) - renderStopwatch.Elapsed;

				if (timeToSleep > TimeSpan.Zero) {
					Thread.Sleep(timeToSleep);
				}
			}

			renderStopwatch.Restart();
		}

		internal void Init()
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			Debug.Log("Loading game...");

			Start();

			Debug.Log("Game started.");
		}

		internal void FixedUpdateInternal()
		{
			fixedUpdate = true;

			GameEngine.FixedUpdate();
		}

		internal void RenderUpdateInternal()
		{
			fixedUpdate = false;

			GameEngine.RenderUpdate();
		}

		internal void ApplicationQuit(object sender, EventArgs e)
		{
			shouldQuit = true;
		}

		private void UpdateLoop()
		{
			ModuleManagement.TryGetModule(out Windowing windowing);

			while (!shouldQuit && (GameEngine.Flags.HasFlag(GameFlags.NoWindow) || windowing?.ShouldClose == false)) {
				Update();
			}
		}

		public static void Quit()
		{
			var instance = Instance;

			if (instance.shouldQuit) {
				return;
			}

			Debug.Log("Game stopping...");

			instance.shouldQuit = true;
		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) // Move this somewhere
		{
#if WINDOWS
			var exception = (Exception)e.ExceptionObject;

			System.Windows.Forms.MessageBox.Show($"{exception.Message}\n\n{exception.StackTrace}", "Error");
#endif

			Quit();
		}
	}
}
