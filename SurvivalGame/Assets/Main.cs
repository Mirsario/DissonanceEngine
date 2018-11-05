using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;
using GameEngine;

#pragma warning disable 219
namespace Game
{
	public static class Program {
		public static void Main() {
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

		//Menu stuff
		private static bool _mainMenu = true;
		public static bool MainMenu {
			get => _mainMenu;
			set {
				showCursor = value;
				lockCursor = !value;
				if(!value) {
					menuState = MenuState.Main;
					prevMenuState = MenuState.Main;
				}
				_mainMenu = value;
			}
		}
		public static MenuState menuState;
		public static MenuState prevMenuState;
		private WorldInfo[] worldList;

		public static GUISkin skin;
		public static string docPath;
		public static string savePath;
		public static string modsPath;
		public static string sourcesPath;
		public static string builtPath;
		private static Entity _localEntity;
		public static Entity LocalEntity {
			get => _localEntity;
			set {
				var oldEntity = _localEntity;
				_localEntity = value;
				if(camera==null) {
					camera = GameObject.Instantiate("Camera").AddComponent<Camera>();
					camera.fov = 110f;
					camera.GameObject.AddComponent<AudioListener>();
					/*camera.OnRenderStart += delegate(Camera cam) {
						if(localEntity!=null && localEntity.renderer!=null) {
							localEntity.renderer.enabled = cam!=camera;
						}
					};*/
				}
				camera.Transform.parent = _localEntity?.Transform;
				_localEntity?.UpdateCamera();

				oldEntity?.UpdateIsPlayer(false);
				_localEntity?.UpdateIsPlayer(true);
			}
		}
		public static Camera camera;
		public static Vector3 cameraRotation;
		public static World world;

		public AudioSource musicSource;
		public static bool enableMusic = false;

		//Input
		public static Vector2 MoveInput => new Vector2(
			(Input.GetKey(Keys.D) ? 1f : 0f)-(Input.GetKey(Keys.A) ? 1f : 0f),
			(Input.GetKey(Keys.W) ? 1f : 0f)-(Input.GetKey(Keys.S) ? 1f : 0f)
		);

		public override void PreInit()
		{
			Texture.defaultFilterMode = FilterMode.Point;// FilterMode.Point;

			Layers.AddLayers(
				"World",
				"Entity"
			);
		}
		public override void Start()
		{
			name = "Incarnate";
			displayName = "Incarnate";
			
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
			
			//World.NewWorld("Test",128,128); //Skips menu
		}
		public override void FixedUpdate()
		{
			if(Input.GetKeyDown(Keys.Escape)) {
				Quit();
			}
			//Physics.gravity = new Vector3(0f,Mathf.Sin(Time.fixedTime*2f)*5f,0f);
		}

		private string worldName = "";
		private static Func<bool?,string> dynamicMenuSetup;
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
							RefreshWorlds().Wait();
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
											RefreshWorlds().Wait();
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
						if(worldName==null || prevMenuState!=menuState) {
							worldName = "";
						}
						GUI.DrawText(new Rect(0,Graphics.ScreenHeight/2-24,Graphics.ScreenWidth,32),"Enter World Name:",TextAlignment.MiddleCenter);
						if(!string.IsNullOrEmpty(Input.InputString)) {
							worldName += Input.InputString;
						}
						if(Input.GetKeyDown(Keys.BackSpace) && worldName.Length>0) {
							worldName = worldName.Remove(worldName.Length-1,1);
						}
						bool showLine = Mathf.FloorToInt(Time.GlobalTime*2f)%2==0;
						GUI.DrawText(new Rect((showLine && worldName.Length>0) ? 6 : 0,Graphics.ScreenHeight*0.5f,Graphics.ScreenWidth,32),worldName+(showLine ? "_" : ""),TextAlignment.MiddleCenter);

						if(GUI.Button(new Rect(Graphics.ScreenWidth/2-128,Graphics.ScreenHeight/2+32,128,64),"Back")) {
							setMenuState = MenuState.WorldSelect;
						}
						bool active=	!string.IsNullOrWhiteSpace(worldName);
						if(GUI.Button(new Rect(Graphics.ScreenWidth*0.5f,Graphics.ScreenHeight/2+32,128,64),"Create",active) || (Input.GetKeyDown(Keys.Enter) && active)) {
							//World.NewWorld(worldName,1024,1024);
							World.NewWorld(worldName,256,256);
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
				GUI.DrawText(new Rect(8,8,128,8),	"Render FPS: "+renderFPS);
				GUI.DrawText(new Rect(8,24,128,8),	"Render MS:  "+renderMs.ToString("0.00"));
				GUI.DrawText(new Rect(8,40,128,8),	"Logic FPS:  "+logicFPS);
				GUI.DrawText(new Rect(8,56,128,8),	"Logic MS:   "+logicMs.ToString("0.00"));
			}
			prevMenuState = menuState;
			if(setMenuState!=null) {
				menuState = setMenuState.Value;
			}

			//GUI.DrawTexture(new Rect(32,32,64,64),TileEntity.tileTexture);	//This was... weirdly moving???
		}
		private async Task RefreshWorlds()
		{
			await Task.Run(() => {
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
			});
		}

		private static void Outdated_StartGame()
		{
			//musicSource = new GameObject("MusicSource").AddComponent<AudioSource>();
			//musicSource.clip = Resources.Get<AudioClip>("testMusic.wav");
			//musicSource.loop = true;
			//if(enableMusic) {
			//	musicSource.Play();
			//}

			var robot = new Robot();
			robot.Transform.Position = new Vector3(11f,1f,10f);
			
			//Entity entity = new Entity();
			//entity.transform.position = new Vector3(6f,4f,6f);
		}
		/*private void CloneTest()
		{
			Material original = World.terrainMaterial;
			Material clone = original.TestClone();
			Debug.Log(clone.shader==original.shader ? "Is exact" : "Is not exact");
			Debug.Log("name:                "+(clone.name==original.name ? "Is exact" : "Not exact"));
			Debug.Log("shader:              "+(clone.shader==original.shader ? "Is exact" : "Not exact"));
			Debug.Log("array:               "+(clone.shaders==original.shaders ? "Is exact" : "Not exact"));
			for(int i=0;i<original.shaders.Length;i++) {
				Debug.Log("array["+i+"]:            "+(original.shaders[i]==clone.shaders[i] ? "Is exact" : "Not exact"));
			}
			Debug.Log("dictionary:          "+(clone._textures==original._textures ? "Is exact" : "Not exact"));
			for(int i=0;i<original._textures.Count;i++) {
				Debug.Log("dictionary["+i+"] key:   "+(original._textures[i].Key==clone._textures[i].Key ? "Is exact" : "Not exact"));
				Debug.Log("dictionary["+i+"] value: "+(original._textures[i].Value==clone._textures[i].Value ? "Is exact" : "Not exact"));
			}
		}*/
	}
}