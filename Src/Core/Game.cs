using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Dissonance.Engine.Core;
using Dissonance.Engine.Graphics;
using Dissonance.Engine.IO;
using Dissonance.Engine.Physics;
using Dissonance.Framework.Imaging;
using Dissonance.Framework.Windowing;

namespace Dissonance.Engine
{
	//TODO: A lot of code here is temporary.
	//TODO: Add animations
	//TODO: Add proper built-in skybox rendering
	//TODO: Redesign resource importing so that one file could output multiple amounts and kinds of assets
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
		public static Dictionary<string,string> filePaths;

		internal static IntPtr window;

		[ThreadStatic]
		private static Game instance;

		public static bool HasFocus	{ get; internal set; } = true;
		public static bool IsFixedUpdate => instance?.fixedUpdate ?? false;

		internal static Game Instance => instance; //TODO: In a perfect world, properties and fields like this one would not exist. But this is not a perfect world.

		internal bool shouldQuit;
		internal bool preInitDone;
		internal bool fixedUpdate;

		public GameFlags Flags { get; private set; }
		public bool NoGraphics { get; private set; }
		public bool NoAudio { get; private set; }

		public void Run(GameFlags flags = GameFlags.None,string[] args = null)
		{
			if(Instance!=null) {
				throw new InvalidOperationException("Cannot run a second Game instance on the same thread. Create a new thread.");
			}

			DllResolver.Init();
			ReflectionCache.Init();
			InitializeModules();

			Debug.Log("Loading engine...");

			Flags = flags;
			NoGraphics = (Flags & GameFlags.NoGraphics)!=0;
			NoAudio = (Flags & GameFlags.NoAudio)!=0;

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

			if(!NoGraphics) {
				PrepareGLFW();
				PrepareGL();
			}

			IL.Init();

			Init();

			UpdateLoop();

			if(!NoGraphics) {
				GLFW.DestroyWindow(window);
				GLFW.Terminate();
			}
		}
		public void Dispose()
		{
			if(!NoGraphics) {
				Rendering.Dispose();
			}

			PhysicsEngine.Dispose();

			instance = null;
		}

		internal void Init()
		{
			Debug.Log($"Working directory is '{Directory.GetCurrentDirectory()}'.");
			Debug.Log($"Assets directory is '{assetsPath}'.");

			Screen.UpdateValues();

			Screen.CursorState = CursorState.Normal;

			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			RenderPass.Init();
			Rand.Init();
			ProgrammableEntityHooks.Initialize();
			GameObject.StaticInit();
			
			if(!Directory.Exists(assetsPath)) {
				throw new DirectoryNotFoundException($"Unable to locate the Assets folder. Is the working directory set correctly?\nExpected it to be '{Path.GetFullPath(assetsPath)}'.");
			}

			filePaths = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);

			foreach(string file in Resources.GetFilesRecursive(assetsPath,null,new[] { $"{assetsPath}/bin",$"{assetsPath}/obj" })) {
				filePaths[Path.GetFileName(file)] = file;
			}
			
			Component.Init();

			CustomVertexBuffer.Initialize();
			CustomVertexAttribute.Initialize();

			Resources.Init();

			if(!NoGraphics) {
				GUI.Init();
			}

			Input.Init();
			PhysicsEngine.Init();

			if(!NoAudio) {
				Audio.Init();
			}

			moduleHooks.Init?.Invoke();

			Debug.Log("Loading game...");

			Start();

			Debug.Log("Game started.");
		}
		internal void FixedUpdateInternal()
		{
			fixedUpdate = true;

			if(shouldQuit) {
				return;
			}

			moduleHooks.PreFixedUpdate?.Invoke();

			bool isFocused = GLFW.GetWindowAttrib(window,WindowAttribute.Focused)!=0;

			/*if(Screen.lockCursor && isFocused) {
				var center = Screen.Center;

				GLFW.SetCursorPos(window,center.x,center.y);
			}*/

			Screen.UpdateValues();
			Input.Update();

			FixedUpdate();

			if(shouldQuit) {
				return;
			}
			
			ProgrammableEntityHooks.InvokeHook(nameof(ProgrammableEntity.FixedUpdate));

			if(shouldQuit) {
				return;
			}

			PhysicsEngine.FixedUpdate();
			Input.LateUpdate();

			if(!NoAudio) {
				Audio.FixedUpdate();
			}

			moduleHooks.PostFixedUpdate?.Invoke();
		}
		internal void RenderUpdateInternal()
		{
			fixedUpdate = false;

			if(shouldQuit) {
				return;
			}

			moduleHooks.PreRenderUpdate?.Invoke();

			Input.Update();

			RenderUpdate();

			if(shouldQuit) {
				return;
			}
			
			ProgrammableEntityHooks.InvokeHook(nameof(ProgrammableEntity.RenderUpdate));

			if(shouldQuit) {
				return;
			}

			PhysicsEngine.RenderUpdate();
			Rendering.Render();
			Input.LateUpdate();

			moduleHooks.PostRenderUpdate?.Invoke();
		}
		internal void ApplicationQuit(object sender,EventArgs e)
		{
			shouldQuit = true;

			instance?.Dispose();
		}

		public virtual void PreInit() {}
		public virtual void Start() {}
		public virtual void FixedUpdate() {}
		public virtual void RenderUpdate() {}
		public virtual void OnGUI() {}
		public virtual void OnApplicationQuit() {}

		private void UpdateLoop()
		{
			Stopwatch updateStopwatch = new Stopwatch();

			updateStopwatch.Start();

			static TimeSpan FrequencyToTimeSpan(double frequency) => new TimeSpan((long)(TimeSpan.TicksPerSecond*(1d/frequency)));

			TimeSpan nextUpdateTime = default;

			while(GLFW.WindowShouldClose(window)==0) {
				GLFW.PollEvents();

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

		public static void Quit() => GLFW.SetWindowShouldClose(window,1);
		
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