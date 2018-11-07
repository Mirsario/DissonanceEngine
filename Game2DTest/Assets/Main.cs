using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;
using GameEngine;

#pragma warning disable 219
namespace Game
{
	public static class Program
	{
		public static void Main()
		{
			using(var main = new Main()) {
				main.Run();
			}
		}
	}
	public class Main : GameEngine.Game
	{
		public const float unitSizeInPixels = 8f;

		public static Camera camera;

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
			name = "2DTest";
			displayName = "2DTest";

			TileType.Initialize();

			camera = GameObject.Instantiate("Camera",new Vector3(0f,0f,10f)).AddComponent<Camera>();
			camera.orthographic = true;
			camera.orthographicSize = Math.Max(Graphics.ScreenWidth,Graphics.ScreenHeight)/32f;

			GameObject.Instantiate<Player>("Player",new Vector3(0f,1f,0f));

			int stone = TileType.GetTypeId<Tiles.Stone>();
			int levelSize = 64;
			int[,] levelTiles = new int[levelSize,levelSize];
			for(int y = 0;y<levelSize;y++) {
				for(int x = 0;x<levelSize;x++) {
					levelTiles[x,y] = (y>levelSize/2 || (y==levelSize/2 && Rand.Next(10)==0)) ? stone :-1;
				}
			}
			var level = new Level(levelTiles);

			/*GameObject.Instantiate<CubeObj>("Cube1",new Vector3(0f,0f,-1f));
			GameObject.Instantiate<CubeObj2>("Cube2",new Vector3(0f,3f,-1f));
			GameObject.Instantiate<CubeObj2>("Cube2",new Vector3(0f,4f,-1f));
			GameObject.Instantiate<CubeObj2>("Cube2",new Vector3(0f,5f,-1f));*/
		}
		public override void FixedUpdate()
		{
			if(Input.GetKeyDown(Keys.Escape)) {
				if(lockCursor) {
					lockCursor = false;
				}else{
					Quit();
				}
			}
			/*float time = Time.GameTime;
			if(Mathf.FloorToInt(time)!=Mathf.FloorToInt(time-Time.DeltaTime)) {
				GameObject.Instantiate<CubeObj2>("Cube2",new Vector3(0.25f,5f,-1f));
			}*/
		}
		public override void OnGUI()
		{
			int y = 8;
			GUI.DrawText(new Rect(8,y,128,8),"Welcome to weird stuff"); y += 16;
			GUI.DrawText(new Rect(8,y,128,8),"Check 'SurvivalGame' project instead"); y += 16;
			GUI.DrawText(new Rect(8,y,128,8),$"Render FPS: {renderFPS}"); y += 16;
			GUI.DrawText(new Rect(8,y,128,8),$"Render MS:  {renderMs.ToString("0.00")}"); y += 16;
			GUI.DrawText(new Rect(8,y,128,8),$"Logic FPS:  {logicFPS}"); y += 16;
			GUI.DrawText(new Rect(8,y,128,8),$"Logic MS:   {logicMs.ToString("0.00")}"); y += 16;
		}
	}
}