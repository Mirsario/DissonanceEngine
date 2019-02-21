using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using GameEngine;
using GameEngine.Graphics;

#pragma warning disable 219
namespace SurvivalGame
{
	public static class Program
	{
		public static void Main()
		{
			var main = new Main();
			main.Run();
		}
	}
	public class Main : Game
	{
		public enum MenuState
		{
			Main,
			WorldSelect,
			WorldCreate,
			DynamicYesOrNo
		}
		
		public static MenuState menuState;
		public static MenuState prevMenuState;
		public static GUISkin skin;
		public static string docPath;
		public static string savePath;
		public static string modsPath;
		public static string sourcesPath;
		public static string builtPath;
		public static Camera camera;
		public static World world;
		public static bool shouldLockCursor;
		private static Func<bool?,string> dynamicMenuSetup;

		#region ToMove
		public static bool enableMusic = false; //shouldn't be here 
		public static Texture whiteTexture; //this too
		#endregion

		private WorldInfo[] worldList;
		private string worldNameString = "";

		private static bool _mainMenu = true;
		public static bool MainMenu {
			get => _mainMenu;
			set {
				shouldLockCursor = !value;
				UpdateCursor();
				if(!value) {
					menuState = MenuState.Main;
					prevMenuState = MenuState.Main;
				}
				_mainMenu = value;
			}
		}
		private static Entity localEntity;
		public static Entity LocalEntity {
			get => localEntity;
			set {
				if(localEntity==value) {
					return;
				}
				camera?.GameObject.Dispose();
				var controllerType = value.CameraControllerType;
				if(controllerType==null || !typeof(CameraController).IsAssignableFrom(controllerType)) {
					throw new Exception($"Invalid CameraControllerType return value, '{controllerType?.ToString() ?? "null"}' does not derive from CameraController class.");
				}
				var controller = (CameraController)GameObject.Instantiate(controllerType,init:false);
				controller.camera = camera = InstantiateCamera(controller);
				controller.entity = value;
				controller.Init();

				localEntity?.UpdateIsPlayer(false);
				value.UpdateIsPlayer(true);
				localEntity = value;
			}
		}
		public static bool EnableFXAA { //Test
			get => Rendering.RenderingPipeline.RenderPasses.FirstOrDefault(r => r.name=="FXAA")?.enabled==true;
			set {
				var pass = Rendering.RenderingPipeline.RenderPasses.FirstOrDefault(r => r.name=="FXAA");
				if(pass!=null) {
					pass.enabled = value;
				}
			}
		}

		public override void PreInit()
		{
			Texture.defaultFilterMode = FilterMode.Point;

			Layers.AddLayers(
				"World",
				"Entity"
			);

			Rendering.SetRenderingPipeline<GameEngine.Graphics.RenderingPipelines.DeferredRendering>();
		}
		public override void Start()
		{
			name = "SurvivalTest";
			displayName = "Survival Test";
			
			docPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace(@"\","/")}/My Games/{name}/"; //TODO: Change for linux and mac
			savePath = docPath+@"Saves/";
			modsPath = docPath+@"Mods/";
			sourcesPath = modsPath+@"Sources/";
			builtPath = modsPath+@"Local/";
			Directory.CreateDirectory(docPath);
			Directory.CreateDirectory(savePath);
			Directory.CreateDirectory(modsPath);
			Directory.CreateDirectory(sourcesPath);
			Directory.CreateDirectory(builtPath);

			whiteTexture = new Texture(1,1);

			Console.BufferHeight = short.MaxValue-1;

			GameInput.Initialize();
		}
		public override void RenderUpdate()
		{
			ScreenShake.StaticRenderUpdate();
		}
		public override void FixedUpdate()
		{
			if(Input.GetKeyDown(Keys.Escape)) {
				if(Screen.lockCursor || !Screen.showCursor) {
					Screen.lockCursor = false;
					Screen.showCursor = true;
				}else{
					Quit();
				}
			}else if(shouldLockCursor && Input.GetMouseButtonDown(MouseButton.Left) || Input.GetMouseButtonDown(MouseButton.Right)) {
				UpdateCursor();
			}
			if(Input.GetKeyDown(Keys.Y)) {
				Debug.Log("Boop");
				EnableFXAA = !EnableFXAA;
			}
			//Physics.gravity = new Vector3(0f,Mathf.Sin(Time.fixedTime*2f)*5f,0f);
		}
		public override void OnGUI()
		{
			MenuState? setMenuState = null;
			if(MainMenu) {
				switch(menuState) {
					case MenuState.Main: {
						if(GUI.Button(new RectFloat(64,Screen.Height-256,256,64),"Play")) {
							setMenuState = MenuState.WorldSelect;
						}
						if(GUI.Button(new RectFloat(64,Screen.Height-192,256,64),"Options")) {
							
						}
						if(GUI.Button(new RectFloat(64,Screen.Height-128,256,64),"Quit")) {
							Quit();
							return;
						}
						break;
					}
					case MenuState.WorldSelect: {
						if(worldList==null || prevMenuState!=menuState) {
							RefreshWorlds();
						}
						if(GUI.Button(new RectFloat(64,Screen.Height-256,256,64),"New World")) {
							setMenuState = MenuState.WorldCreate;
						}
						if(GUI.Button(new RectFloat(64,Screen.Height-192,256,64),"Back")) {
							setMenuState = MenuState.Main;
						}
						GUI.DrawText(new RectFloat(0,Screen.Height/2-272,Screen.Width,32),"Worlds",alignment:TextAlignment.MiddleCenter);
						for(int i=0;i<worldList.Length;i++) {
							if(GUI.Button(new RectFloat((Screen.Width*0.5f)-256,256+(i*64),448,64),worldList[i].displayName)) {
								World.LoadWorld(worldList[i].localPath);
							}
							if(GUI.Button(new RectFloat((Screen.Width*0.5f)+192,256+(i*64),64,64),"X")) {
								int iCopy = i;
								setMenuState = MenuState.DynamicYesOrNo;
								dynamicMenuSetup = result => {
									if(result!=null) {
										if(result==true) {
											File.Delete(worldList[iCopy].localPath);
											RefreshWorlds();
										}
										menuState = MenuState.WorldSelect;
									}
									return "Are you sure you want to delete this world?";
								};
							}
						}
						break;
					}
					case MenuState.WorldCreate: {
						if(worldNameString==null || prevMenuState!=menuState) {
							worldNameString = "";
						}
						GUI.DrawText(new RectFloat(0,Screen.Height/2-24,Screen.Width,32),"Enter World Name:",alignment:TextAlignment.MiddleCenter);
						if(!string.IsNullOrEmpty(Input.InputString)) {
							worldNameString += Input.InputString;
						}
						if(Input.GetKeyDown(Keys.BackSpace) && worldNameString.Length>0) {
							worldNameString = worldNameString.Remove(worldNameString.Length-1,1);
						}
						bool showLine = Mathf.FloorToInt(Time.GlobalTime*2f)%2==0;
						GUI.DrawText(new RectFloat((showLine && worldNameString.Length>0) ? 6 : 0,Screen.Height*0.5f,Screen.Width,32),worldNameString+(showLine ? "_" : ""),alignment:TextAlignment.MiddleCenter);

						if(GUI.Button(new RectFloat(Screen.Width/2-128,Screen.Height/2+32,128,64),"Back")) {
							setMenuState = MenuState.WorldSelect;
						}
						bool active=	!string.IsNullOrWhiteSpace(worldNameString);
						if(GUI.Button(new RectFloat(Screen.Width*0.5f,Screen.Height/2+32,128,64),"Create",active) || (Input.GetKeyDown(Keys.Enter) && active)) {
							world = World.NewWorld(worldNameString,256,256);
							setMenuState = MenuState.Main;
						}
						break;
					}
					case MenuState.DynamicYesOrNo: {
						bool? result = null;
						if(GUI.Button(new RectFloat(Screen.Width/2-128,Screen.Height/2+16,128,64),"Yes")) {
							result = true;
						}
						if(GUI.Button(new RectFloat(Screen.Width*0.5f,Screen.Height/2+16,128,64),"No")) {
							result = false;
						}
						string text = dynamicMenuSetup(result);
						GUI.DrawText(new RectFloat(0,Screen.Height/2-16,Screen.Width,32),text,alignment:TextAlignment.MiddleCenter);
						break;
					}
				}
			}else{
				int i = 0;
				GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),"Render FPS: "+renderFPS);
				GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),"Render MS: "+renderMs.ToString("0.00"));
				GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),"Logic FPS: "+logicFPS);
				GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),"Logic MS: "+logicMs.ToString("0.00"));
				GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),$"FXAA: {(EnableFXAA ? "Enabled" : "Disabled")} ([Y] - Toggle)");
			}
			prevMenuState = menuState;
			if(setMenuState!=null) {
				menuState = setMenuState.Value;
			}

			//GUI.DrawTexture(new Rect(32,32,64,64),TileEntity.tileTexture);	//This was... weirdly moving???
		}

		private void RefreshWorlds()
		{
			//await Task.Run(() => {
			var files = Directory.GetFiles(savePath,"*.wld");
			var worlds = new List<WorldInfo>();
			for(int i=0;i<files.Length;i++) {
				try {
					var reader = new BinaryReader(File.OpenRead(files[i]));
					if(World.ReadInfoHeader(reader,out var info)) {
						reader.Close();
						worlds.Add(new WorldInfo {
							name = info.name,
							displayName = info.displayName,
							xSize = info.xSize,
							ySize = info.ySize,
							localPath = files[i]
						});
						continue;
					}
					reader.Close();
				}
				catch {
					worlds.Add(new WorldInfo {
						name = "CorruptWorld",
						displayName = $"Corrupt World ({Path.GetFileName(files[i])})",
					});
				}
			}
			worldList = worlds.ToArray();
			//});
		}

		public static Camera InstantiateCamera(CameraController controller)
		{
			var newCamera = controller.AddComponent<Camera>();
			newCamera.fov = 110f;
			controller.AddComponent<AudioListener>();
			return newCamera;
		}
		public static void UpdateCursor()
		{
			Screen.lockCursor = shouldLockCursor;
			Screen.showCursor = !shouldLockCursor;
		}
	}
}