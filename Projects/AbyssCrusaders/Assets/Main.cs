using System.IO;
using System;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Graphics.RenderingPipelines;
using AbyssCrusaders.UI.Menu;

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

		public override void PreInit()
		{
			Texture.defaultFilterMode = FilterMode.Point;
			Texture.defaultWrapMode = TextureWrapMode.Clamp;
			Layers.AddLayers(
				"World",
				"Entity"
			);
			Time.TargetRenderCount = 144;
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
			var skybox = GameObject.Instantiate<Skybox>();

			Rendering.ambientColor = new Vector3(0f,0f,0f);

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
		public override void OnGUI()
		{
			if(mainMenu && menuController!=null) {
				menuController.OnGUI();
			}else{
				float y = 0.5f;
				GUI.DrawText(new RectFloat(8,y++*16,128,8),$"Render FPS: {renderFPS }");
				GUI.DrawText(new RectFloat(8,y++*16,128,8),$"Render MS: {renderMs.ToString("0.00")}");
				GUI.DrawText(new RectFloat(8,y++*16,128,8),$"Logic FPS: {logicFPS}");
				GUI.DrawText(new RectFloat(8,y++*16,128,8),$"Logic MS: {logicMs.ToString("0.00")}");
				GUI.DrawText(new RectFloat(8,y++*16,128,8),$"Draw Calls Count: {Rendering.drawCallsCount}");
			}
		}
	}
}