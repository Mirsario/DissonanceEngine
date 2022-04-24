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
		public static Thread MainThread { get; private set; }

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

		/// <summary>
		/// The <see cref="GameFlags"/> the game was ran with.
		/// </summary>
		public GameFlags Flags { get; private set; }
		public IReadOnlyList<string> StartArguments { get; private set; }

		internal bool NoWindow { get; private set; }
		internal bool NoGraphics { get; private set; }
		internal bool NoAudio { get; private set; }

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
			MainThread = Thread.CurrentThread;
			Flags = flags;
			StartArguments = Array.AsReadOnly(args ?? Array.Empty<string>());
			NoWindow = Flags.HasFlag(GameFlags.NoWindow);
			NoGraphics = Flags.HasFlag(GameFlags.NoGraphics);
			NoAudio = Flags.HasFlag(GameFlags.NoAudio);

			AppDomain.CurrentDomain.ProcessExit += ApplicationQuit;

			PreInit();

			GameEngine.Initialize();

			Debug.Log($"Working directory is '{Directory.GetCurrentDirectory()}'.");

			Init();

			if (!Flags.HasFlag(GameFlags.ManualUpdate)) {
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
				if (!NoWindow) {
					GlfwApi.GLFW.PollEvents();
				}

				FixedUpdateInternal();

				numFixedUpdates++;
			}

			if (NoGraphics) {
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

			while (!shouldQuit && (NoWindow || windowing?.ShouldClose == false)) {
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

		private static void OnFocusChange(IntPtr _, int isFocused)
			=> HasFocus = isFocused != 0;

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) // Move this somewhere
		{
#if WINDOWS
			var exception = (Exception)e.ExceptionObject;

			System.Windows.Forms.MessageBox.Show(exception.Message+"\n\n"+exception.StackTrace,"Error");
#endif

			Quit();
		}
	}
}
