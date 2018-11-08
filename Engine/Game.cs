using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace GameEngine
{
	//TODO: Finish .smartmesh format
	//TODO: Add animations
	//TODO: Add submeshes
	//TODO: Add proper built-in skybox rendering
	//TODO: Redesign resource importing so that one file could output multiple amounts and kinds of assets
	//TODO: Add occlusion culling
	//TODO: Add proper toggling between fullscreen, windowed fullscreen and normal windowed modes
	//TODO: Fix issues with window resizing
	public class Game : IDisposable
	{
		//Debug
		private const bool bigScreen = true;
		private const int defaultWidth = bigScreen ? 1280 : 960; //1600;	
		private const int defaultHeight = bigScreen ? 720 : 540; //960;	
		
		internal static Game instance;
		internal static bool shouldQuit;
		internal static bool preInitDone;
		internal static bool fixedUpdate;
		/*#if DEBUG
		internal static FileSystemWatcher fileWatcher;
		internal DateTime lastWriteDate = DateTime.MinValue;
		#endif*/

		public static int targetUpdates = 60;
		public static int targetFPS = 0;
		public static string name = "UntitledGame";
		public static string displayName = "Untitled Game";
		public static string assetsPath;
		public static bool lockCursor;
		public static bool showCursor;
		public static Dictionary<string,string> filePaths;
		public static GameWindow window;

		//FPS counter
		private static Stopwatch logicStopwatch;
		public static int logicFrame;
		public static int logicFPS;
		public static float logicMs;
		private static float logicMsTemp;
		//
		public static Stopwatch renderStopwatch;
		public static int renderFrame;
		public static int renderFPS;
		public static float renderMs;
		private static float renderMsTemp;

		public static bool HasFocus	{ get; internal set; } = true;

		public void Run()
		{
			Console.BufferHeight = short.MaxValue-1;
			assetsPath = "Assets"+Path.DirectorySeparatorChar;

			Layers.Init();
			//... Load gameconfig.json here
			Time.PreInit();
			PreInit();
			preInitDone = true;

			window = new GameWindow(defaultWidth,defaultHeight,GraphicsMode.Default,displayName,GameWindowFlags.FixedWindow);
			Graphics.window = window;
			
			window.VSync = VSyncMode.Off;
			
			window.Load += (obj,e) => Init();
			window.Resize += Graphics.Resize;
			window.UpdateFrame += FixedUpdateInternal;
			window.RenderFrame += RenderUpdateInternal;
			window.KeyUp += Input.KeyUp;
			window.KeyPress += Input.KeyPress;
			window.KeyDown += Input.KeyDown;
			window.MouseMove += Input.MouseMove;
			window.FocusedChanged += OnFocusChange;
			window.Run(Time.targetUpdateCount,Time.targetRenderCount);
		}

		internal void Init()
		{
			Debug.Log("Loading engine...");

			Debug.Log($"Working directory is '{Directory.GetCurrentDirectory()}'");
			//AppDomain.CurrentDomain.AppendPrivatePath("References");
			AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = "/References/";

			logicStopwatch = new Stopwatch();
			renderStopwatch = new Stopwatch();
			lockCursor = false;
			showCursor = true;

			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			instance = this;
			Extensions.Init();
			ReflectionCache.Init();
			RenderPass.Init();
			Rand.Init();
			GameObject.StaticInit();

			
			if(!Directory.Exists("Assets")) {
				throw new DirectoryNotFoundException("Unable to locate the Assets folder. Is the working directory set correctly?");
			}
			filePaths = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
			foreach(string file in Resources.GetFilesRecursive(@"Assets\",null,new[] { @"Assets\bin",@"Assets\obj" })) {
				filePaths[Path.GetFileName(file)] = file;
			}

			#region FileUpdating
			//TODO: Implement resource reloading
			/*#if DEBUG
			//Experimental, not finished yet.
			fileWatcher = new FileSystemWatcher {
				Path = "Assets",
				NotifyFilter = NotifyFilters.LastWrite,
				Filter = "*.*"
			};
			fileWatcher.Changed += delegate(object obj,FileSystemEventArgs args) {
				DateTime lastWriteDateNew = File.GetLastWriteTime(args.FullPath);
				if(lastWriteDate.Ticks!=lastWriteDateNew.Ticks) {
					Debug.Log("old: "+lastWriteDate.Ticks+",new: "+lastWriteDateNew.Ticks);
					Debug.Log("old: "+lastWriteDate+",new: "+lastWriteDateNew);
					lastWriteDate = lastWriteDateNew;
					Debug.Log(args.FullPath+" was changed");
				}
			};
			fileWatcher.EnableRaisingEvents = true;
			#endif*/
			#endregion
			
			Component.Init();
			Resources.Init();
			Graphics.Init();
			GUI.Init();
			Input.Init();
			Time.Init();
			Physics.Init();
			Audio.Init();

			Debug.Log("Loading game...");
            Start();
			Debug.Log("Game started.");
		}
		internal void FixedUpdateInternal(object sender,FrameEventArgs e)
		{
			fixedUpdate = true;
			logicStopwatch.Restart();
			if(lockCursor && window.Focused) {
				var center = Graphics.WindowCenter;
				Mouse.SetPosition(center.x,center.y);
			}
            window.CursorVisible = showCursor || !window.Focused;

			Time.UpdateFixed(1.0/targetUpdates);
			Input.FixedUpdate();
			
			Physics.Update();
			FixedUpdate();
			if(shouldQuit) { return; }
			
			for(int i=0;i<GameObject.gameObjects.Count;i++) {
				var obj = GameObject.gameObjects[i];
				foreach(var pair in obj.components) {
					for(int j=0;j<pair.Value.Count;j++) {
						pair.Value[j].FixedUpdate();
					}
				}
				obj.FixedUpdate();
			}
			if(shouldQuit) { return; }

			Physics.UpdateFixed();
			Input.LateFixedUpdate();
			Audio.FixedUpdate();

			MeasureFPS(ref logicFPS,ref logicFrame,Time.fixedTime,Time.fixedTimePrev,logicStopwatch,ref logicMs,ref logicMsTemp);
		}
		internal void RenderUpdateInternal(object sender,FrameEventArgs e)
		{
			fixedUpdate = false;
			if(shouldQuit) { return; }
			renderStopwatch.Restart();

			Time.UpdateRender(e.Time);
			Input.RenderUpdate();

			RenderUpdate();
			if(shouldQuit) { return; }
			
			for(int i=0;i<GameObject.gameObjects.Count;i++) {
				var obj = GameObject.gameObjects[i];
				foreach(var pair in obj.components) {
					for(int j=0;j<pair.Value.Count;j++) {
						pair.Value[j].RenderUpdate();
					}
				}
				obj.RenderUpdate();
			}
			if(shouldQuit) { return; }

			Physics.UpdateRender();
			Graphics.Render();
			Input.LateRenderUpdate();

			MeasureFPS(ref renderFPS,ref renderFrame,Time.renderTime,Time.renderTimePrev,renderStopwatch,ref renderMs,ref renderMsTemp);
		}
		
		public void Dispose()
		{
			Graphics.Dispose();
			Physics.Dispose();
		}

		#region VirtualMethods
		public virtual void PreInit() {}
		public virtual void Start() {}
		public virtual void FixedUpdate() {}
		public virtual void RenderUpdate() {}
		public virtual void OnGUI() {}
		#endregion

		#region StaticMethods
		internal static void MeasureFPS(ref int fps,ref int frames,float time,float timePrev,Stopwatch stopwatch,ref float ms,ref float msTemp)
		{
			msTemp += stopwatch.ElapsedMilliseconds;
			frames++;
			if(Mathf.FloorToInt(time)>Mathf.FloorToInt(timePrev)) {
				fps = frames;
				frames = 0;
				ms = msTemp/Math.Max(1,fps);
				msTemp = 0f;
			}
		}
		private static void OnFocusChange(object sender,EventArgs e)
		{
			HasFocus = window.Focused;
		}
		private static void OnUnhandledException(object sender,UnhandledExceptionEventArgs e)
		{
			var exception = (Exception)e.ExceptionObject;
			System.Windows.Forms.MessageBox.Show(exception.Message+"\n\n"+exception.StackTrace,"Error");
			Quit();	
		}

		public static void Quit()
		{
			shouldQuit = true;
			window.Exit();
			instance.Dispose();
		}
		#endregion
	}
}