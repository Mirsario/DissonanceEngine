using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Dissonance.Framework;
using Dissonance.Framework.GLFW3;
using GameEngine.Graphics;
using GameEngine.Physics;
using GameEngine.Core;
using Dissonance.Framework.OpenGL;

namespace GameEngine
{
	//TODO: Finish .smartmesh format
	//TODO: Add animations
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

		public static string name = "UntitledGame";
		public static string displayName = "Untitled Game";
		public static string assetsPath;
		public static Dictionary<string,string> filePaths;
		public static IntPtr window;
		
		internal static Game instance;
		internal static bool shouldQuit;
		internal static bool preInitDone;
		internal static bool fixedUpdate;

		public static bool HasFocus	{ get; internal set; } = true;

		public void Run(string[] args = null)
		{
			Console.BufferHeight = short.MaxValue-1;

			assetsPath = "Assets"+Path.DirectorySeparatorChar;

			DllResolver.Init();

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
			Time.PreInit();
			Rendering.PreInit();
			PreInit();

			preInitDone = true;

			GLFW.SetErrorCallback((GLFWError code,string description) => Console.WriteLine(code switch {
				GLFWError.VersionUnavailable => throw new GraphicsException(description),
				_ => $"GLFW Error {code}: {description}"
			}));

			if(GLFW.Init()==0) {
				throw new Exception("Unable to initialize GLFW!");
			}

			GLFW.WindowHint(WindowHint.ContextVersionMajor,3); //Targeted major version
			GLFW.WindowHint(WindowHint.ContextVersionMinor,2); //Targeted minor version

			window = GLFW.CreateWindow(800,600,"Unnamed Window",IntPtr.Zero,IntPtr.Zero);

			GLFW.MakeContextCurrent(window);

			GLFW.SetWindowFocusCallback(window,OnFocusChange);

			GL.Load();

			Init();

			while(GLFW.WindowShouldClose(window)==0) {
				GLFW.PollEvents();

				FixedUpdateInternal();
				RenderUpdateInternal();
			}

			GLFW.DestroyWindow(window);
			GLFW.Terminate();

			//Rendering.window = window = new GameWindow(DefaultWidth,DefaultHeight,GraphicsMode.Default,displayName); //,GameWindowFlags.Default,DisplayDevice.Default,1,0,GraphicsContextFlags.Default,null,false);

			//window.VSync = VSyncMode.On;

			/*window.Load += (obj,e) => Init();
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

			window.Run(Time.TargetUpdateFrequency,Time.TargetRenderFrequency);*/
		}
		public void Dispose()
		{
			Rendering.Dispose();
			//PhysicsEngine.Dispose();
		}

		internal void Init()
		{
			Debug.Log("Loading engine...");

			Debug.Log($"Working directory is '{Directory.GetCurrentDirectory()}'.");
			Debug.Log($"Assets directory is '{assetsPath}'.");
			//AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = "/References/";

			Time.Init();
			Screen.UpdateValues();

			Screen.lockCursor = false;
			Screen.showCursor = true;

			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			instance = this;

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
			
			Component.Init();
			Resources.Init();
			Rendering.Init();
			GUI.Init();
			Input.Init();
			//PhysicsEngine.Init();
			Audio.Init();

			Debug.Log("Loading game...");

			Start();

			Debug.Log("Game started.");
		}
		internal void FixedUpdateInternal()
		{
			fixedUpdate = true;

			Time.PreFixedUpdate();

			bool isFocused = GLFW.GetWindowAttrib(window,WindowAttribute.Focused)!=0;

			if(Screen.lockCursor && isFocused) {
				var center = Screen.WindowCenter;

				GLFW.SetCursorPos(Game.window,center.x,center.y);
			}

			Screen.CursorVisible = Screen.showCursor || !isFocused;

			Screen.UpdateValues();

			Time.UpdateFixed(1.0/Time.TargetUpdateFrequency);
			Input.FixedUpdate();

			FixedUpdate();

			if(shouldQuit) {
				return;
			}
			
			ProgrammableEntityHooks.InvokeHook(nameof(ProgrammableEntity.FixedUpdate));

			if(shouldQuit) {
				return;
			}

			//PhysicsEngine.UpdateFixed();
			Input.LateFixedUpdate();
			Audio.FixedUpdate();

			Time.PostFixedUpdate();
		}
		internal void RenderUpdateInternal()
		{
			fixedUpdate = false;

			if(shouldQuit) {
				return;
			}

			Time.PreRenderUpdate();

			Time.UpdateRender(GLFW.GetTime());
			Input.RenderUpdate();

			RenderUpdate();

			if(shouldQuit) {
				return;
			}
			
			ProgrammableEntityHooks.InvokeHook(nameof(ProgrammableEntity.RenderUpdate));

			if(shouldQuit) {
				return;
			}

			//PhysicsEngine.UpdateRender();
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

		public static void Quit() => GLFW.SetWindowShouldClose(window,1);
		
		private static void OnFocusChange(IntPtr window,int isFocused) => HasFocus = isFocused!=0;
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