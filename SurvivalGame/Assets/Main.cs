using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System;
using GameEngine;

#pragma warning disable 219
namespace Game
{
	public static class Program
	{
		public static void Main()
		{
			var main = new Main();
			main.Run();
		}
	}
	public class Main : GameEngine.Game
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
		public static Vector3 cameraRotation;
		public static World world;
		public static bool enableMusic = false; //shouldn't be here 
		public static bool shouldLockCursor;
		private static Func<bool?,string> dynamicMenuSetup;

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
		private static Mob localMob;
		public static Mob LocalMob {
			get => localMob;
			set {
				var oldEntity = localMob;
				localMob = value;
				if(camera==null) {
					camera = Entity.Instantiate<Entity>(localMob.world,"Camera").AddComponent<Camera>();
					camera.fov = 110f;
					camera.GameObject.AddComponent<AudioListener>();
				}
				camera.Transform.parent = localMob?.Transform;
				localMob?.UpdateCamera();

				oldEntity?.UpdateIsPlayer(false);
				localMob?.UpdateIsPlayer(true);
			}
		}
		public static bool EnableFXAA { //Test
			get => Graphics.renderSettings.renderPasses.FirstOrDefault(r => r.name=="fxaa")?.enabled==true;
			set {
				var pass = Graphics.renderSettings.renderPasses.FirstOrDefault(r => r.name=="fxaa");
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

			Console.BufferHeight = short.MaxValue-1;
		}
		public override void FixedUpdate()
		{
			if(Input.GetKeyDown(Keys.Escape)) {
				if(lockCursor || !showCursor) {
					lockCursor = false;
					showCursor = true;
				}else{
					Quit();
				}
			}else if(shouldLockCursor && Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
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
						if(GUI.Button(new Rect(64,Graphics.ScreenHeight-256,256,64),"Play")) {
							setMenuState = MenuState.WorldSelect;
						}
						if(GUI.Button(new Rect(64,Graphics.ScreenHeight-192,256,64),"Options")) {
							
						}
						if(GUI.Button(new Rect(64,Graphics.ScreenHeight-128,256,64),"Quit")) {
							Quit();
							return;
						}
						break;
					}
					case MenuState.WorldSelect: {
						if(worldList==null || prevMenuState!=menuState) {
							RefreshWorlds();
						}
						if(GUI.Button(new Rect(64,Graphics.ScreenHeight-256,256,64),"New World")) {
							setMenuState = MenuState.WorldCreate;
						}
						if(GUI.Button(new Rect(64,Graphics.ScreenHeight-192,256,64),"Back")) {
							setMenuState = MenuState.Main;
						}
						GUI.DrawText(new Rect(0,Graphics.ScreenHeight/2-272,Graphics.ScreenWidth,32),"Worlds",TextAlignment.MiddleCenter);
						for(int i=0;i<worldList.Length;i++) {
							if(GUI.Button(new Rect((Graphics.ScreenWidth*0.5f)-256,256+(i*64),448,64),worldList[i].displayName)) {
								World.LoadWorld(worldList[i].localPath);
							}
							if(GUI.Button(new Rect((Graphics.ScreenWidth*0.5f)+192,256+(i*64),64,64),"X")) {
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
						GUI.DrawText(new Rect(0,Graphics.ScreenHeight/2-24,Graphics.ScreenWidth,32),"Enter World Name:",TextAlignment.MiddleCenter);
						if(!string.IsNullOrEmpty(Input.InputString)) {
							worldNameString += Input.InputString;
						}
						if(Input.GetKeyDown(Keys.BackSpace) && worldNameString.Length>0) {
							worldNameString = worldNameString.Remove(worldNameString.Length-1,1);
						}
						bool showLine = Mathf.FloorToInt(Time.GlobalTime*2f)%2==0;
						GUI.DrawText(new Rect((showLine && worldNameString.Length>0) ? 6 : 0,Graphics.ScreenHeight*0.5f,Graphics.ScreenWidth,32),worldNameString+(showLine ? "_" : ""),TextAlignment.MiddleCenter);

						if(GUI.Button(new Rect(Graphics.ScreenWidth/2-128,Graphics.ScreenHeight/2+32,128,64),"Back")) {
							setMenuState = MenuState.WorldSelect;
						}
						bool active=	!string.IsNullOrWhiteSpace(worldNameString);
						if(GUI.Button(new Rect(Graphics.ScreenWidth*0.5f,Graphics.ScreenHeight/2+32,128,64),"Create",active) || (Input.GetKeyDown(Keys.Enter) && active)) {
							//world = World.NewWorld(worldName,1024,1024);
							world = World.NewWorld(worldNameString,256,256);
							setMenuState = MenuState.Main;
						}
						break;
					}
					case MenuState.DynamicYesOrNo: {
						bool? result = null;
						if(GUI.Button(new Rect(Graphics.ScreenWidth/2-128,Graphics.ScreenHeight/2+16,128,64),"Yes")) {
							result = true;
						}
						if(GUI.Button(new Rect(Graphics.ScreenWidth*0.5f,Graphics.ScreenHeight/2+16,128,64),"No")) {
							result = false;
						}
						string text = dynamicMenuSetup(result);
						GUI.DrawText(new Rect(0,Graphics.ScreenHeight/2-16,Graphics.ScreenWidth,32),text,TextAlignment.MiddleCenter);
						break;
					}
				}
			}else{
				int i = 0;
				GUI.DrawText(new Rect(8,8+(i++*16),128,8),"Render FPS: "+renderFPS);
				GUI.DrawText(new Rect(8,8+(i++*16),128,8),"Render MS: "+renderMs.ToString("0.00"));
				GUI.DrawText(new Rect(8,8+(i++*16),128,8),"Logic FPS: "+logicFPS);
				GUI.DrawText(new Rect(8,8+(i++*16),128,8),"Logic MS: "+logicMs.ToString("0.00"));
				GUI.DrawText(new Rect(8,8+(i++*16),128,8),$"FXAA: {(EnableFXAA ? "Enabled" : "Disabled")} ([Y] - Toggle)");
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

		public static void UpdateCursor()
		{
			lockCursor = shouldLockCursor;
			showCursor = !shouldLockCursor;
		}
	}
}