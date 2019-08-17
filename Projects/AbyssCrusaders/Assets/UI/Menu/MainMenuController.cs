using GameEngine;

namespace AbyssCrusaders.UI.Menu
{
	public class MainMenuController : MenuController
	{
		public override void OnGUI()
		{ 
			GUI.DrawText(new RectFloat(Screen.Width*0.5f-48f,Screen.Height*0.1f,128f,24f),"game.",alignment:TextAlignment.MiddleCenter,fontSize:32);
			GUI.DrawText(new RectFloat(8f,Screen.Height-24f,128f,24f),"Version 0.0.BANANA");

			base.OnGUI();
		}
	}
}
