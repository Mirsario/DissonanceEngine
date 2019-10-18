using GameEngine;

namespace AbyssCrusaders.UI.Menu
{
	public class WorldCreate : MenuState
	{
		//public const int WorldWidth = 4096;
		//public const int WorldHeight = 4096;
		public const int WorldWidth = 1024;
		public const int WorldHeight = 1024;
		public string worldName;
		
		public override void OnActivated()
		{
			worldName = "";
		}
		public override void OnGUI()
		{
			GUI.DrawText(new RectFloat(0,Screen.Height/2-24,Screen.Width,32),"Enter World Name:",alignment:TextAlignment.MiddleCenter);

			//Handle typing
			if(!string.IsNullOrEmpty(Input.InputString)) {
				worldName += Input.InputString;
			}

			if(Input.GetKeyDown(Keys.BackSpace) && worldName.Length>0) {
				worldName = worldName.Remove(worldName.Length-1,1);
			}

			bool showLine = Mathf.FloorToInt(Time.GlobalTime*2f)%2==0;
			GUI.DrawText(new RectFloat((showLine && worldName.Length>0) ? 6 : 0,Screen.Height*0.5f,Screen.Width,32),worldName+(showLine ? "_" : ""),alignment:TextAlignment.MiddleCenter);

			//Create Button
			bool active = !string.IsNullOrWhiteSpace(worldName);
			if(GUI.Button(new RectFloat(Screen.Width*0.5f,Screen.Height/2+32,128,64),"Create",active) || (Input.GetKeyDown(Keys.Enter) && active)) {
				var world = World.Create<Overworld>(WorldWidth,WorldHeight);

				var player = Entity.Instantiate<Player>(world,"Player",(Vector2)world.spawnPoint);
				Main.camera.followObject = player;

				controller.SetState<MainMenuState>();
				Main.mainMenu = false;
			}

			//Back Button
			if(GUI.Button(new RectFloat(Screen.Width/2-128,Screen.Height/2+32,128,64),"Back")) {
				controller.SetState<WorldSelect>();
			}
		}
	}
}
