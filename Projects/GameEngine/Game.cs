using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using GameEngine.Graphics;

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
		private const bool BigScreen = true;

		internal const int DefaultWidth = BigScreen ? 1600 : 960; //1600;	
		internal const int DefaultHeight = BigScreen ? 900 : 540; //960;	

		public static int targetUpdates = 60;
		public static int targetFPS = 0;
		public static string name = "UntitledGame";
		public static string displayName = "Untitled Game";
		public static string assetsPath;
		public static Dictionary<string,string> filePaths;
		public static GameWindow window;
		
		internal static Game instance;
		internal static bool shouldQuit;
		internal static bool preInitDone;
		internal static bool fixedUpdate;

		public static bool HasFocus	{ get; internal set; } = true;

		public void Run(string[] args = null)
		{
			Console.BufferHeight = short.MaxValue-1;
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

			Layers.Init();
			//... Load gameconfig.json here
			Time.PreInit();
			Rendering.PreInit();
			PreInit();
			preInitDone = true;

			//var device = DisplayDevice.Default;
			Rendering.window = window = new GameWindow(DefaultWidth,DefaultHeight,GraphicsMode.Default,displayName); //,GameWindowFlags.Default,DisplayDevice.Default,1,0,GraphicsContextFlags.Default,null,false);
			
			window.VSync = VSyncMode.Off;
			
			window.Load += (obj,e) => Init();
			window.Resize += Rendering.Resize;
			window.WindowStateChanged += (sender,e) => {
				Debug.Log("State changed");
				Rendering.Resize(sender,e);
			};
			window.UpdateFrame += FixedUpdateInternal;
			window.RenderFrame += RenderUpdateInternal;
			window.KeyUp += Input.KeyUp;
			window.KeyPress += Input.KeyPress;
			window.KeyDown += Input.KeyDown;
			window.MouseUp += Input.MouseUp;
			window.MouseDown += Input.MouseDown;
			window.FocusedChanged += OnFocusChange;
			window.Closing += ApplicationQuit;

			window.Run(Time.targetUpdateCount,Time.targetRenderCount);
		}
		public void Dispose()
		{
			Rendering.Dispose();
			Physics.Dispose();
		}

		internal void Init()
		{
			Debug.Log("Loading engine...");

			Debug.Log($"Working directory is '{Directory.GetCurrentDirectory()}'.");
			Debug.Log($"Assets directory is '{assetsPath}'.");
			AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = "/References/";

			Time.Init();
			Screen.UpdateValues(window);
			Screen.lockCursor = false;
			Screen.showCursor = true;

			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			instance = this;
			Extensions.Init();
			ReflectionCache.Init();
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
			Rendering.Init();
			GUI.Init();
			Input.Init();
			Physics.Init();
			Audio.Init();

			Debug.Log("Loading game...");
            Start();
			Debug.Log("Game started.");
		}
		internal void FixedUpdateInternal(object sender,FrameEventArgs e)
		{
			fixedUpdate = true;
			Time.PreFixedUpdate();

			if(Screen.lockCursor && window.Focused) {
				var center = Screen.WindowCenter;
				Mouse.SetPosition(center.x,center.y);
			}

            window.CursorVisible = Screen.showCursor || !window.Focused;
			Screen.UpdateValues(window);

			Time.UpdateFixed(1.0/targetUpdates);
			Input.FixedUpdate();
			
			Physics.Update();
			FixedUpdate();

			if(shouldQuit) {
				return;
			}
			
			ProgrammableEntityHooks.InvokeHook(nameof(ProgrammableEntity.FixedUpdate));

			if(shouldQuit) {
				return;
			}

			Physics.UpdateFixed();
			Input.LateFixedUpdate();
			Audio.FixedUpdate();

			Time.PostFixedUpdate();
		}
		internal void RenderUpdateInternal(object sender,FrameEventArgs e)
		{
			/*//TODO: This is a temporary fix for what seems to be an OpenTK bug, which introduces issues with setting GL.Viewport's width & height to values bigger than window's original Width & Height. There must be a better solution.
			if(!resizedWindow) {
				window.Location = new System.Drawing.Point((window.Width/2)-(DefaultWidth/2),(window.Height/2)-(DefaultHeight/2));
				window.Size = new System.Drawing.Size(DefaultWidth,DefaultHeight);
				Screen.UpdateValues(window);
				resizedWindow = true;
			}*/
			fixedUpdate = false;

			if(shouldQuit) {
				return;
			}

			Time.PreRenderUpdate();

			Time.UpdateRender(e.Time);
			Input.RenderUpdate();

			RenderUpdate();

			if(shouldQuit) {
				return;
			}
			
			ProgrammableEntityHooks.InvokeHook(nameof(ProgrammableEntity.RenderUpdate));

			if(shouldQuit) {
				return;
			}

			Physics.UpdateRender();
			Rendering.Render();
			Input.LateRenderUpdate();

			Time.PostRenderUpdate();
		}
		internal void ApplicationQuit(object sender,CancelEventArgs e)
		{
			shouldQuit = true;
			instance.Dispose();
		}

		public virtual void PreInit() {}
		public virtual void Start() {}
		public virtual void FixedUpdate() {}
		public virtual void RenderUpdate() {}
		public virtual void OnGUI() {}
		public virtual void OnApplicationQuit() {}

		public static void Quit() => window.Exit();
		
		private static void OnFocusChange(object sender,EventArgs e) => HasFocus = window.Focused;
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