using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Dissonance.Engine.Graphics;
using Dissonance.Framework.Windowing;

namespace Dissonance.Engine
{
	//TODO: Add animations
	//TODO: Add proper built-in skybox rendering
	//TODO: Add occlusion culling
	//TODO: Add proper toggling between fullscreen, windowed fullscreen and normal windowed modes
	public partial class Game : IDisposable
	{
		//Debug
		private const bool BigScreen = true;

		internal const int DefaultWidth = BigScreen ? 1600 : 960; //1600;
		internal const int DefaultHeight = BigScreen ? 900 : 540; //960;

		private static readonly List<Game> InstancesList;

		private static volatile Game globalInstance;
		private static volatile bool multipleInstances;
		[ThreadStatic]
		private static Game threadStaticInstance;

		public static bool HasFocus { get; internal set; } = true;
		public static bool IsFixedUpdate => Instance?.fixedUpdate ?? false;

		public static Game Instance {
			get {
				if(multipleInstances) {
					return threadStaticInstance ?? throw new InvalidOperationException($"Multiple Game instances were created, but there is no instance associated with the current thread.");
				}

				return globalInstance ?? throw new InvalidOperationException($"No active Game instance currently exists.");
			}
		}

		public static IReadOnlyList<Game> Instances;

		internal bool shouldQuit;
		internal bool preInitDone;
		internal bool fixedUpdate;
		internal string assetsPath;

		private Stopwatch updateStopwatch;
		private ulong numFixedUpdates;

		public string Name { get; set; } = "UntitledGame";
		public string DisplayName { get; set; } = "Untitled Game";
		public GameFlags Flags { get; private set; }

		internal bool NoWindow { get; private set; }
		internal bool NoGraphics { get; private set; }
		internal bool NoAudio { get; private set; }

		static Game()
		{
			Instances = (InstancesList = new List<Game>()).AsReadOnly();
		}

		public virtual void PreInit() { }
		public virtual void Start() { }
		public virtual void FixedUpdate() { }
		public virtual void RenderUpdate() { }
		public virtual void OnGUI() { }
		public virtual void OnDispose() { }

		public void Run(GameFlags flags = GameFlags.None, string[] args = null)
		{
			RegisterInstance();

			Flags = flags;
			NoWindow = Flags.HasFlag(GameFlags.NoWindow);
			NoGraphics = Flags.HasFlag(GameFlags.NoGraphics);
			NoAudio = Flags.HasFlag(GameFlags.NoAudio);

			DllResolver.Init();
			AssemblyCache.Init();
			InitializeModules();

			Debug.Log("Loading engine...");

			//TODO: Move this.
			assetsPath = "Assets" + Path.DirectorySeparatorChar;

			if(args != null) {
				string joinedArgs = string.Join(" ", args);
				var matches = RegexCache.commandArguments.Matches(joinedArgs);
				var dict = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

				foreach(Match match in matches) {
					dict[match.Groups[1].Value] = match.Groups[2].Value;
				}

				if(dict.TryGetValue("assetspath", out string path)) {
					assetsPath = path ?? throw new ArgumentException("Expected a directory path after command line argument 'assetspath'.");
				}
			}

			assetsPath = Path.GetFullPath(assetsPath);

			AppDomain.CurrentDomain.ProcessExit += ApplicationQuit;

			moduleHooks.PreInit?.Invoke();
			PreInit();

			preInitDone = true;

			Init();

			if(!Flags.HasFlag(GameFlags.ManualUpdate)) {
				UpdateLoop();
				Dispose();
			}
		}
		public void Dispose()
		{
			OnDispose();

			if(modules != null) {
				for(int i = 0; i < modules.Count; i++) {
					modules[i]?.Dispose();
				}

				modules = null;
			}

			threadStaticInstance = null;

			if(globalInstance == this) {
				globalInstance = null;
			}
		}
		public void Update()
		{
			if(updateStopwatch == null) {
				updateStopwatch = new Stopwatch();

				updateStopwatch.Start();
			}

			while(numFixedUpdates < (ulong)Math.Floor(updateStopwatch.Elapsed.TotalSeconds * Time.TargetUpdateFrequency)) {
				if(!NoWindow) {
					GLFW.PollEvents();
				}

				FixedUpdateInternal();

				numFixedUpdates++;
			}

			if(!NoGraphics) {
				RenderUpdateInternal();
			}
		}
		public void AssociateWithCurrentThread() => threadStaticInstance = this;

		internal void Init()
		{
			Debug.Log($"Working directory is '{Directory.GetCurrentDirectory()}'.");
			Debug.Log($"Assets directory is '{assetsPath}'.");

			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			RenderPass.Init();

			if(!Directory.Exists(assetsPath)) {
				throw new DirectoryNotFoundException($"Unable to locate the Assets folder. Is the working directory set correctly?\nExpected it to be '{Path.GetFullPath(assetsPath)}'.");
			}

			CustomVertexBuffer.Initialize();
			CustomVertexAttribute.Initialize();

			moduleHooks.Init?.Invoke();

			Debug.Log("Loading game...");

			Start();

			Debug.Log("Game started.");
		}
		internal void FixedUpdateInternal()
		{
			fixedUpdate = true;

			moduleHooks.PreFixedUpdate?.Invoke();

			/*bool isFocused = GLFW.GetWindowAttrib(window,WindowAttribute.Focused)!=0;

			if(Screen.lockCursor && isFocused) {
				var center = Screen.Center;

				GLFW.SetCursorPos(window,center.x,center.y);
			}*/

			FixedUpdate();
			ProgrammableEntityManager.InvokeHook(nameof(ProgrammableEntity.FixedUpdate));
			moduleHooks.FixedUpdate?.Invoke();

			moduleHooks.PostFixedUpdate?.Invoke();
		}
		internal void RenderUpdateInternal()
		{
			fixedUpdate = false;

			moduleHooks.PreRenderUpdate?.Invoke();

			RenderUpdate();
			ProgrammableEntityManager.InvokeHook(nameof(ProgrammableEntity.RenderUpdate));
			moduleHooks.RenderUpdate?.Invoke();

			moduleHooks.PostRenderUpdate?.Invoke();
		}
		internal void ApplicationQuit(object sender, EventArgs e)
		{
			shouldQuit = true;

			threadStaticInstance?.Dispose();
		}

		private void RegisterInstance()
		{
			if(threadStaticInstance != null) {
				throw new InvalidOperationException("Cannot run a second Game instance on the same thread. Create it in a new thread.");
			}

			lock(InstancesList) {
				InstancesList.Add(this);

				globalInstance ??= this;
				threadStaticInstance = this;
				multipleInstances = InstancesList.Count > 1;
			}
		}
		private void UpdateLoop()
		{
			var windowing = GetModule<Windowing>(false);

			while(!shouldQuit && (NoWindow || !windowing.ShouldClose)) {
				Update();
			}
		}

		public static void Quit()
		{
			var instance = Instance;

			if(instance.shouldQuit) {
				return;
			}

			Debug.Log("Game stopping...");

			instance.shouldQuit = true;
		}

		private static void OnFocusChange(IntPtr _, int isFocused) => HasFocus = isFocused != 0;
		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) //Move this somewhere
		{
#if WINDOWS
			
			var exception = (Exception)e.ExceptionObject;

			System.Windows.Forms.MessageBox.Show(exception.Message+"\n\n"+exception.StackTrace,"Error");
			
#endif

			Quit();
		}
	}
}
