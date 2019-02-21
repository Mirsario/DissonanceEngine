using GameEngine;

namespace AbyssCrusaders.UI.Menu
{
	public class MainMenuState : MenuState
	{
		public override void OnGUI()
		{
			Vector2Int screenHalf = (Vector2Int)(Screen.Size*0.5f);
			const float ButtonWidth = 256f;
			const float ButtonHeight = 64f;
			const float ButtonWidthHalf = ButtonWidth*0.5f;
			const float ButtonHeightHalf = ButtonHeight*0.5f;
			float yStep = -1.5f;

			bool Button(string text) => GUI.Button(new RectFloat(screenHalf.x-ButtonWidthHalf,screenHalf.y+(yStep++*ButtonHeight)-ButtonHeightHalf,ButtonWidth,64),text);

			if(Button("Singleplayer")) {
				controller.SetState<WorldSelect>();
			}

			if(Button("Multiplayer")) {
				controller.SetState<WorldSelect>();
			}

			Button("Settings");

			if(Button("Quit")) {
				Game.Quit();
				return;
			}
		}
	}
}
