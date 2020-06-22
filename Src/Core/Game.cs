using System;
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

		public static string name = "UntitledGame";
		public static string displayName = "Untitled Game";
		public static string assetsPath;

		internal static IntPtr window;

		[ThreadStatic]
		private static Game instance;

		public static bool HasFocus { get; internal set; } = true;
		public static bool IsFixedUpdate => instance?.fixedUpdate ?? false;

		internal static Game Instance => instance; //TODO: In a perfect world, properties and fields like this one would not exist. But this is not a perfect world.

		internal bool shouldQuit;
		internal bool preInitDone;
		internal bool fixedUpdate;

		public GameFlags Flags { get; private set; }
		public bool NoGraphics { get; private set; }
		public bool NoAudio { get; private set; }

		public virtual void PreInit() { }
		public virtual void Start() { }
		public virtual void FixedUpdate() { }
		public virtual void RenderUpdate() { }
		public virtual void OnGUI() { }
		public virtual void OnApplicationQuit() { }

		public void Run(GameFlags flags = GameFlags.None,string[] args = null)
		{
			if(Instance!=null) {
				throw new InvalidOperationException("Cannot run a second Game instance on the same thread. Create a new thread.");
			}

			Flags = flags;
			NoGraphics = (Flags & GameFlags.NoGraphics)!=0;
			NoAudio = (Flags & GameFlags.NoAudio)!=0;

			DllResolver.Init();
			ReflectionCache.Init();
			InitializeModules();

			Debug.Log("Loading engine...");

			instance = this;
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

			if(!NoGraphics) {
				GLFW.DestroyWindow(window);
				GLFW.Terminate();
			}
		}
		public void Dispose()
		{
			if(modules!=null) {
				for(int i = 0;i<modules.Count;i++) {
					modules[i]?.Dispose();
				}

				modules = null;
			}

			instance = null;
		}

		internal void Init()
		{
			Debug.Log($"Working directory is '{Directory.GetCurrentDirectory()}'.");
			Debug.Log($"Assets directory is '{assetsPath}'.");

			Screen.CursorState = CursorState.Normal;

			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			RenderPass.Init();
			GameObject.StaticInit();

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

			instance?.Dispose();
		}

		private void UpdateLoop()
		{
			Stopwatch updateStopwatch = new Stopwatch();

			updateStopwatch.Start();

			static TimeSpan FrequencyToTimeSpan(double frequency) => new TimeSpan((long)(TimeSpan.TicksPerSecond*(1d/frequency)));

			TimeSpan nextUpdateTime = default;

			while(!shouldQuit && (NoGraphics || GLFW.WindowShouldClose(window)==0)) {
				if(!NoGraphics) {
					GLFW.PollEvents();
				}

				FixedUpdateInternal();

				if(!NoGraphics) {
					RenderUpdateInternal();
				}

				TimeSpan sleepTimeSpan = nextUpdateTime-updateStopwatch.Elapsed;

				if(sleepTimeSpan>TimeSpan.Zero) {
					Thread.Sleep(sleepTimeSpan);
				}

				nextUpdateTime = updateStopwatch.Elapsed+FrequencyToTimeSpan(Time.TargetUpdateFrequency);
			}
		}

		public static void Quit()
		{
			Instance.shouldQuit = true;

			if(window!=IntPtr.Zero) {
				GLFW.SetWindowShouldClose(window,1);
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