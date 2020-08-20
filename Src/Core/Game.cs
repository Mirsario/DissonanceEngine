using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Dissonance.Engine.Core.Internal;
using Dissonance.Engine.Core.ProgrammableEntities;
using Dissonance.Engine.Graphics;
using Dissonance.Engine.Graphics.Meshes.Buffers;
using Dissonance.Engine.Graphics.Meshes.VertexAttributes;
using Dissonance.Engine.Graphics.RenderPasses;
using Dissonance.Framework.Windowing;

namespace Dissonance.Engine.Core
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

		private static readonly ConcurrentQueue<Game> Instances = new ConcurrentQueue<Game>();

		public static string name = "UntitledGame";
		public static string displayName = "Untitled Game";
		public static string assetsPath;

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

		internal bool shouldQuit;
		internal bool preInitDone;
		internal bool fixedUpdate;

		public GameFlags Flags { get; private set; }
		public bool NoWindow { get; private set; }
		public bool NoAudio { get; private set; }

		public virtual void PreInit() { }
		public virtual void Start() { }
		public virtual void FixedUpdate() { }
		public virtual void RenderUpdate() { }
		public virtual void OnGUI() { }
		public virtual void OnDispose() { }

		public void Run(GameFlags flags = GameFlags.Default,string[] args = null)
		{
			RegisterInstance();

			Flags = flags;
			NoWindow = !Flags.HasFlag(GameFlags.Graphics);
			NoAudio = !Flags.HasFlag(GameFlags.Audio);

			DllResolver.Init();
			AssemblyCache.Init();
			InitializeModules();

			Debug.Log("Loading engine...");

			//TODO: Move this.
			assetsPath = "Assets"+Path.DirectorySeparatorChar;

			if(args!=null) {
				string joinedArgs = string.Join(" ",args);
				var matches = RegexCache.commandArguments.Matches(joinedArgs);
				var dict = new Dictionary<string,string>(StringComparer.InvariantCultureIgnoreCase);

				foreach(Match match in matches) {
					dict[match.Groups[1].Value] = match.Groups[2].Value;
				}

				if(dict.TryGetValue("assetspath",out string path)) {
					assetsPath = path ?? throw new ArgumentException("Expected a directory path after command line argument 'assetspath'.");
				}
			}

			assetsPath = Path.GetFullPath(assetsPath);

			AppDomain.CurrentDomain.ProcessExit += ApplicationQuit;

			moduleHooks.PreInit?.Invoke();
			PreInit();

			preInitDone = true;

			Init();

			UpdateLoop();

			if(!NoWindow && TryGetModule<Windowing>(out var windowing)) {
				GLFW.DestroyWindow(windowing.WindowHandle);
				GLFW.Terminate();
			}

			Dispose();
		}
		public void Dispose()
		{
			OnDispose();

			if(modules!=null) {
				for(int i = 0;i<modules.Count;i++) {
					modules[i]?.Dispose();
				}

				modules = null;
			}

			threadStaticInstance = null;

			if(globalInstance==this) {
				globalInstance = null;
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
		internal void ApplicationQuit(object sender,EventArgs e)
		{
			shouldQuit = true;

			threadStaticInstance?.Dispose();
		}

		private void RegisterInstance()
		{
			if(threadStaticInstance!=null) {
				throw new InvalidOperationException("Cannot run a second Game instance on the same thread. Create it in a new thread.");
			}

			Instances.Enqueue(this);

			globalInstance ??= this;
			threadStaticInstance = this;
			multipleInstances = Instances.Count>1;
		}
		private void UpdateLoop()
		{
			Stopwatch updateStopwatch = new Stopwatch();

			updateStopwatch.Start();

			var windowing = GetModule<Windowing>(false);
			int numFixedUpdates = 0;

			while(!shouldQuit && (NoWindow || GLFW.WindowShouldClose(windowing.WindowHandle)==0)) {
				while(numFixedUpdates<(int)Math.Floor(updateStopwatch.Elapsed.TotalSeconds*Time.TargetUpdateFrequency)) {
					if(!NoWindow) {
						GLFW.PollEvents();
					}

					FixedUpdateInternal();

					numFixedUpdates++;
				}

				if(!NoWindow) {
					RenderUpdateInternal();
				}
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

			if(instance.TryGetModule<Windowing>(out var windowing) && windowing.WindowHandle!=IntPtr.Zero) {
				GLFW.SetWindowShouldClose(windowing.WindowHandle,1);
			}
		}

		private static void OnFocusChange(IntPtr _,int isFocused) => HasFocus = isFocused!=0;
		private static void OnUnhandledException(object sender,UnhandledExceptionEventArgs e) //Move this somewhere
		{
#if WINDOWS
			
			var exception = (Exception)e.ExceptionObject;

			System.Windows.Forms.MessageBox.Show(exception.Message+"\n\n"+exception.StackTrace,"Error");
			
#endif

			Quit();
		}
	}
}