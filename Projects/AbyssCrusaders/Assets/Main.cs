using System.IO;
using System;
using GameEngine;
using GameEngine.Graphics;
using AbyssCrusaders.UI.Menu;
using GameEngine.Physics;
using System.Collections.Generic;

#pragma warning disable 219
namespace AbyssCrusaders
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			using(var main = new Main()) {
				main.Run(args);
			}
		}
	}
	public class Main : Game
	{
		public const int UnitSizeInPixels = 10;
		public const float PixelSizeInUnits = 1f/UnitSizeInPixels;
		
		public static string docPath;
		public static string savePath;
		public static CameraObj camera;
		public static MenuController menuController;
		public static bool mainMenu;
		public static Dictionary<string,string> debugStrings = new Dictionary<string,string>();

		public static Vector2 MouseWorld { get; private set; }

		public override void PreInit()
		{
			Texture.defaultFilterMode = FilterMode.Point;
			Texture.defaultWrapMode = TextureWrapMode.Clamp;
			Layers.AddLayers(
				"Terrain",
				"TerrainLightingOcclusion",
				"Entity"
			);
			//Time.TargetRenderCount = 144;
			Rendering.SetRenderingPipeline<CustomRenderPipeline>(); //ForwardRendering
		}
		public override void Start()
		{
			name = "2DTest";
			displayName = "2DTest";
			mainMenu = true;

			docPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace(@"\","/")}/My Games/{name}/"; //TODO: Change for linux and mac
			savePath = docPath+@"Saves/";
			Directory.CreateDirectory(docPath);
			Directory.CreateDirectory(savePath);

			GameInput.Initialize();
			TileFrameset.Initialize();
			TilePreset.Initialize();

			menuController = new MainMenuController();
			menuController.SetState<MainMenuState>();

			camera = GameObject.Instantiate<CameraObj>("Camera",new Vector3(0f,0f,10f));
			GameObject.Instantiate<Skybox>();

			Rendering.ambientColor = new Vector3(0.02f,0.02f,0.02f);

			/*var world = World.Create<Overworld>(4096,2048); //World.Create<Overworld>(8192,4096);
			var player = Entity.Instantiate<Player>(world,"Player",(Vector2)world.spawnPoint);
			camera = GameObject.Instantiate<CameraObj>("Camera",new Vector3(world.spawnPoint.x,-world.spawnPoint.y,10f));
			camera.followObject = player;
			var skybox = GameObject.Instantiate<Skybox>();*/
		}
		public override void FixedUpdate()
		{
			if(Input.GetKeyDown(Keys.Escape)) {
				if(Screen.lockCursor) {
					Screen.lockCursor = false;
				}else{
					Quit();
				}
			}
		}
		public override void RenderUpdate()
		{
			if(camera!=null) {
				MouseWorld = camera.Position+((Input.MousePosition-Screen.Size*0.5f)/camera.zoom*PixelSizeInUnits);
			}
		}
		public override void OnGUI()
		{
			if(mainMenu && menuController!=null) {
				menuController.OnGUI();
			}else{
				float y = 0.5f;
				RectFloat Rect() => new RectFloat(8,y++*16,128,8);

				GUI.DrawText(Rect(),$"Render FPS: {Time.RenderFramerate }");
				GUI.DrawText(Rect(),$"Render MS: {Time.RenderMs:0.00}");
				GUI.DrawText(Rect(),$"Logic FPS: {Time.FixedFramerate}");
				GUI.DrawText(Rect(),$"Logic MS: {Time.FixedMs:0.00}");
				GUI.DrawText(Rect(),$"Draw Calls Count: {Rendering.drawCallsCount}");

				y++;

				foreach(var pair in debugStrings) {
					GUI.DrawText(Rect(),$"{pair.Key}: {pair.Value}");
				}

				debugStrings.Clear();
			}
		}
	}
}